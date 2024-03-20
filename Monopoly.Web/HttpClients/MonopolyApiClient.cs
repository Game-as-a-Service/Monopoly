using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Client.HttpClients;

public class MonopolyApiClient(HttpClient httpClient)
{
    public async Task<IEnumerable<PlayerModel>> GetPlayers()
    {
        var response = await httpClient.GetAsync("/users");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<PlayerModel>>() ?? [];
    }
    
    public async Task<IEnumerable<string>> GetRooms()
    {
        var response = await httpClient.GetAsync("/rooms");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<string>>() ?? [];
    }
    
    public async Task CreateGame(string hostToken, IEnumerable<PlayerModel> players)
    {
        var payload = new
        {
            Players = players.ToArray()
        };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hostToken);
        var response = await httpClient.PostAsJsonAsync("/games", payload);
        response.EnsureSuccessStatusCode();
    }
}

public class PlayerModel
{
    public required string Id { get; set; }
    public required string Token { get; set; }
}