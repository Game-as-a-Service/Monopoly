using Microsoft.AspNetCore.SignalR.Client;
using SharedLibrary.ResponseArgs;

namespace Client.Pages;

internal class TypedHubConnection
{
    private readonly HubConnection hubConnection;

    public event PlayerJoinGameEventDelegate? PlayerJoinGameEventHandler;
    public delegate void PlayerJoinGameEventDelegate(PlayerJoinGameEvent e);
    public event GetReadyInfoEventDelegate? GetReadyInfoEventHandler;
    public delegate void GetReadyInfoEventDelegate(GetReadyInfoEvent e);
    public event WelcomeEventDelegate? WelcomeEventHandler;
    public delegate void WelcomeEventDelegate(WelcomeEvent e);

    public TypedHubConnection(HubConnection hubConnection)
    {
        hubConnection.On<PlayerJoinGameEvent>(nameof(PlayerJoinGameEvent), (e) => PlayerJoinGameEventHandler?.Invoke(e));
        hubConnection.On<GetReadyInfoEvent>(nameof(GetReadyInfoEvent), (e) => GetReadyInfoEventHandler?.Invoke(e));
        hubConnection.On<WelcomeEvent>(nameof(WelcomeEvent), (e) => WelcomeEventHandler?.Invoke(e));
        this.hubConnection = hubConnection;
    }
    public async Task GetReadyInfo() => await hubConnection.SendAsync("GetReadyInfo");
}

internal class PlayerJoinGameEvent : EventArgs
{
    public required string PlayerId { get; set; }
}
