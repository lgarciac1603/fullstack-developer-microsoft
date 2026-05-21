using System.Text;
using System.Text.Json;

public class TokenService
{
    private readonly HttpClient _httpClient;

    public TokenService (HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetTokenAsync()
    {
        var request = new StringContent(
            JsonSerializer.Serialize(new { email = "eve.holt@reques.in", password = "cityslicks"}),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("https://reques.in/api/login", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("token").GetString();
    }
}