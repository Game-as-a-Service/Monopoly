using Client.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using MudBlazor;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Client.Pages;

public partial class Index
{
    private List<User> Users { get; set; } = new();
    private string GameId { get; set; } = string.Empty;
    private readonly List<GameData> games = new();
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IOptions<BackendApiOptions> BackendApiOptions { get; set; } = default!;
    private Uri BackendApiBaseUri => new(BackendApiOptions.Value.BaseUrl);

    protected override async Task OnInitializedAsync()
    {
        var tokens = await new HttpClient().GetFromJsonAsync<string[]>(new Uri(BackendApiBaseUri, "/tokens"));
        if (tokens is null)
        {
            Snackbar.Add("取得 Token 失敗", Severity.Error);
            return;
        }

        foreach (var token in tokens)
        {
            AddUser(token);
        }
        await base.OnInitializedAsync();
    }

    private async void CreateGame()
    {
        CreateGameBodyPayload bodyPayload = new(Users.Select(user => new Player(user.Id)).ToArray());
        var url = new Uri(BackendApiBaseUri, "/create-game");
        var httpClient = new HttpClient();
        var host = Users.FirstOrDefault();
        if (host is null)
        {
            Snackbar.Add("請先加入使用者", Severity.Error);
            return;
        }
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", host.Token);
        var response = await httpClient.PostAsJsonAsync(url, bodyPayload);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Snackbar.Add($"遊戲建立成功! Url: {content}", Severity.Success);
        }
        else
        {
            Snackbar.Add($"遊戲建立失敗! {response.StatusCode}", Severity.Error);
        }
    }

    private async void AddUser(string token)
    {
        var url = new Uri(BackendApiBaseUri, "/whoami");
        HubConnection hubConnection = new HubConnectionBuilder()
            .WithAutomaticReconnect()
            .WithUrl(url, opt =>
            {
                opt.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .Build();
        hubConnection.Closed += async (exception) =>
        {
            if (exception == null)
            {
                Snackbar.Add("中斷連線", Severity.Error);
            }
            else
            {
                Snackbar.Add($"中斷連線: {exception.Message}", Severity.Error);
            }
            await Task.CompletedTask;
        };
        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        hubConnection.On<List<string>>("WhoAmI", data =>
        {
            var userId = data.Single(d => d.StartsWith(ClaimTypes.Sid))[(ClaimTypes.Sid.Length + 1)..];
            tcs.SetResult(userId);
        });

        try
        {
            await hubConnection.StartAsync();
            Snackbar.Add("連線成功!", Severity.Success);
            await hubConnection.SendAsync("WhoAmI");
            var tcst = await tcs.Task;
            Users.Add(new(tcst, token));
            await hubConnection.StopAsync();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private void JoinGame()
    {
        games.Clear();
        Users.ForEach(user =>
        {
            games.Add(new(GameId, user.Token));
        });
        StateHasChanged();
    }

    private record CreateGameBodyPayload(Player[] Players);
    private record Player(string Id);

    record GameData(string Id, string Token);
    record User(string Id, string Token);
}
