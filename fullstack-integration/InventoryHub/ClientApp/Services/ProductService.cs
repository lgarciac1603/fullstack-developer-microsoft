using System.Net.Http.Json;
using ClientApp.Models;

namespace ClientApp.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Product>>
                ("api/products")
                ?? new List<Product>();
        }
        catch
        {
            return new List<Product>();
        }
    }
}
