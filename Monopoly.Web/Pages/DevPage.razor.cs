using Client.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Client.Pages;

public partial class DevPage
{
    private IEnumerable<Player>? players = [];
    private IEnumerable<Room>? rooms = [];
    [Inject] private IOptions<BackendApiOptions> BackendApiOptions { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    private Uri BackendApiBaseUri => new(BackendApiOptions.Value.BaseUrl);

    protected override async Task OnInitializedAsync()
    {
        var users = await new HttpClient().GetFromJsonAsync<Player[]>(new Uri(BackendApiBaseUri, "/users"));
        players = users?.Select(p => new Player(p.Id, p.Token));
    }

    private async void CreateGame()
    {
        CreateGameBodyPayload bodyPayload = new([.. players]);
        var url = new Uri(BackendApiBaseUri, "/games");
        var httpClient = new HttpClient();
        var host = players?.FirstOrDefault();
        if (host is null)
        {
            //Snackbar.Add("請先加入使用者", Severity.Error);
            return;
        }
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", host.Token);
        var response = await httpClient.PostAsJsonAsync(url, bodyPayload);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            //Snackbar.Add($"遊戲建立成功! Url: {content}", Severity.Normal);
            await RefleshRoomListAsync();
        }
        else
        {
            //Snackbar.Add($"遊戲建立失敗! {response.StatusCode}", Severity.Error);
        }
    }

    private async Task RefleshRoomListAsync()
    {
        var roomIds = await new HttpClient().GetFromJsonAsync<List<string>>(new Uri(BackendApiBaseUri, "/rooms"));
        rooms = roomIds?.Select(id => new Room(id, [.. players]));
        StateHasChanged();
    }
    private void EnterRoom(Room room, Player player)
    {
        NavigationManager.NavigateTo($"games/{room.Id}?token={player.Token}");
    }

    private record CreateGameBodyPayload(Player[] Players);
    private record Player(string Id, string Token);
    record Room(string Id, IEnumerable<Player> Players);
}
