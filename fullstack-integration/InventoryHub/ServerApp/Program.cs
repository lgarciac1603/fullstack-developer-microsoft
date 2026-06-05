using ServerApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

var products = new List<Product>
{
    new()
    {
        Id = 1,
        Name = "Laptop",
        Price = 1200.50m,
        Stock = 25
    },
    new()
    {
        Id = 2,
        Name = "Headphones",
        Price = 50.00m,
        Stock = 100
    },
    new()
    {
        Id = 3,
        Name = "Keyboard",
        Price = 75.00m,
        Stock = 40
    }
};

app.MapGet("/api/products", () =>
{
    return Results.Ok(products);
});

app.Run();
