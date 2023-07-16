using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace Client.Pages;

public partial class Game
{
    [Parameter] public string Id { get; set; }
    [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private HubConnection? hubConnection;
    private readonly List<string> messages = new();

    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl($"https://localhost:3826/monopoly?gameid={Id}", options =>
            {
                options.AccessTokenProvider = () =>
                {
                    return LocalStorageService.GetItemAsync<string?>("JWT").AsTask();
                };
            })
            .Build();
        try
        {
            await hubConnection.StartAsync();
            Snackbar.Add("連線成功!", Severity.Success);
            SetupHubConnection();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
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
        };
    }
}