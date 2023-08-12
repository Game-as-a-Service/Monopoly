using Client.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using SharedLibrary;
using System.Net.Http.Json;

namespace Client.Pages;

public partial class Game
{
    [Parameter] public string Id { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "access_token")]
    public string AccessToken { get; set; } = default!;

    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private HubConnection hubConnection = default!;
    private readonly List<string> messages = new();
    private BlazorMap? blazorMap;


    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl($"https://localhost:3826/monopoly?gameid={Id}", options =>
            {
                options.AccessTokenProvider = async () => await Task.FromResult(AccessToken);
            })
            .Build();
        try
        {
            SetupHubConnection();
            await hubConnection.StartAsync();
            Snackbar.Add("連線成功!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        await GetMapFromApiAsync("1");
    }

    private async Task GetMapFromApiAsync(string id)
    {
        // map 從 Server 取得
        var httpclient = new HttpClient();
        var response = await httpclient.GetAsync($"https://localhost:3826/map?mapid={id}");
        if (!response.IsSuccessStatusCode)
        {
            Snackbar.Add($"取得地圖失敗: {response.StatusCode}", Severity.Error);
            return;
        }
        var monopolyMap = await response.Content.ReadFromJsonAsync<MonopolyMap>(MonopolyMap.JsonSerializerOptions);
        blazorMap = new BlazorMap(monopolyMap!);
        StateHasChanged();
    }

    private void SetupHubConnection()
    {
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
        hubConnection.On<string>("PlayerJoinGameEvent", id =>
        {
            Snackbar.Add($"player {id} joined game!", Severity.Success);
            messages.Add($"player {id} joined game!");
            StateHasChanged();
        });
    }
}