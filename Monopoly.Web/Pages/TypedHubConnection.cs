using Microsoft.AspNetCore.SignalR.Client;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace Client.Pages;

internal class TypedHubConnection
{
    private readonly HubConnection hubConnection;

    public event GetReadyInfoEventDelegate? GetReadyInfoEventHandler;
    public delegate void GetReadyInfoEventDelegate(GetReadyInfoEventArgs e);
    public event WelcomeEventDelegate? WelcomeEventHandler;
    public delegate void WelcomeEventDelegate(WelcomeEventArgs e);

    public TypedHubConnection(HubConnection hubConnection)
    {
        hubConnection.On<GetReadyInfoEventArgs>("GetReadyInfoEvent", (e) => GetReadyInfoEventHandler?.Invoke(e));
        hubConnection.On<WelcomeEventArgs>("WelcomeEvent", (e) => WelcomeEventHandler?.Invoke(e));
        this.hubConnection = hubConnection;
    }
    public async Task GetReadyInfo() => await hubConnection.SendAsync("GetReadyInfo");
}
