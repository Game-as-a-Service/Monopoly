using Client.Components;
using Client.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using MudBlazor;
using SharedLibrary;
using SharedLibrary.MonopolyMap;
using System.Net.Http.Json;

namespace Client.Pages;

public partial class Game
{
    [Parameter] public string Id { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "token")]
    public string AccessToken { get; set; } = default!;

    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IOptions<BackendApiOptions> BackendApiOptions { get; set; } = default!;
    private Uri BackendApiBaseUri => new(BackendApiOptions.Value.BaseUrl);

    private HubConnection hubConnection = default!;
    private readonly List<string> messages = new();
    private BlazorMap? blazorMap;


    protected override async Task OnInitializedAsync()
    {
        var url = new Uri(BackendApiBaseUri, $"/monopoly?gameid={Id}");
        hubConnection = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
                options.AccessTokenProvider = async () => await Task.FromResult(AccessToken);
            })
            .Build();
        SetupHubConnection();
        try
        {
            await hubConnection.StartAsync();
            Snackbar.Add("連線成功!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        await hubConnection.SendAsync("GetGameStatus");
        await GetMapFromApiAsync("1");
    }

    private async Task GetMapFromApiAsync(string id)
    {
        var url = new Uri(BackendApiBaseUri, $"/map?mapid={id}");
        // map 從 Server 取得
        var response = await new HttpClient().GetAsync(url);
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
            var errorMessage = exception?.Message;
            Snackbar.Add($"中斷連線: {errorMessage}", Severity.Error);
            await Task.CompletedTask;
        };
        
        TypedHubConnection<IMonopolyResponses> connection = new(hubConnection);

        connection.On(x => x.PlayerJoinGameEvent, (string id) =>
        {
            Snackbar.Add($"player {id} joined game!", Severity.Success);
            messages.Add($"player {id} joined game!");
            StateHasChanged();
        });
    }
}