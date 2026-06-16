using LogiTrack.Data;
using LogiTrack.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========================================================
// CONFIGURACIÓN DE SERVICIOS
// ========================================================

// 1. Configurar DbContext con SQLite
builder.Services.AddDbContext<LogiTrackContext>(options =>
    options.UseSqlite("Data Source=logitrack.db"));

// 2. Configurar Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<LogiTrackContext>()
.AddDefaultTokenProviders();

// 3. Configurar JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
    "LogiTrackSuperSecretKey12345678901234567890");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "LogiTrackAPI",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "LogiTrackClients",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// 4. Agregar MemoryCache
builder.Services.AddMemoryCache();

// 5. Agregar Controladores
builder.Services.AddControllers();

// 6. Configurar Swagger (VERSIÓN CORREGIDA)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LogiTrack API",
        Version = "v1",
        Description = "Sistema de gestión de órdenes e inventario"
    });

    // Configurar autenticación JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// 7. Agregar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ========================================================
// PIPELINE DE MIDDLEWARE
// ========================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ========================================================
// INICIALIZACIÓN DE DATOS
// ========================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LogiTrackContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Asegurar que la base de datos esté creada
        await context.Database.EnsureCreatedAsync();

        // Crear roles
        string[] roles = { "Admin", "Manager", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($"Rol creado: {role}");
            }
        }

        // Crear usuario admin
        var adminEmail = "admin@logitrack.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Administrador Principal",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(newAdmin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
                await userManager.AddToRoleAsync(newAdmin, "Manager");
                Console.WriteLine($"Usuario admin creado: {adminEmail}");
            }
        }

        // Datos de prueba - inventario
        if (!context.InventoryItems.Any())
        {
            var items = new[]
            {
                new InventoryItem { Name = "Pallet Jack", Quantity = 12, Location = "Warehouse A", Category = "Equipment" },
                new InventoryItem { Name = "Forklift", Quantity = 5, Location = "Warehouse B", Category = "Equipment" },
                new InventoryItem { Name = "Packing Tape", Quantity = 50, Location = "Warehouse A", Category = "Supplies" },
                new InventoryItem { Name = "Shipping Boxes (Large)", Quantity = 100, Location = "Warehouse C", Category = "Packaging" },
                new InventoryItem { Name = "Shipping Boxes (Small)", Quantity = 200, Location = "Warehouse C", Category = "Packaging" },
                new InventoryItem { Name = "Label Printer", Quantity = 8, Location = "Warehouse A", Category = "Equipment" }
            };

            context.InventoryItems.AddRange(items);
            await context.SaveChangesAsync();
            Console.WriteLine($"{items.Length} items de prueba creados");
        }

        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("LOGITRACK API INICIADA");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Admin: {adminEmail} / Admin123!");
        Console.WriteLine($"Items en inventario: {context.InventoryItems.Count()}");
        Console.WriteLine($"Órdenes: {context.Orders.Count()}");
        Console.WriteLine($"Usuarios: {userManager.Users.Count()}");
        Console.WriteLine(new string('=', 50));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar: {ex.Message}");
    }
}

app.Run();