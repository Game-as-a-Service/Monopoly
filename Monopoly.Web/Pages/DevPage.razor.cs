using Client.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Client.HttpClients;

namespace Client.Pages;

public partial class DevPage
{
    private IEnumerable<Player> _players = [];
    private IEnumerable<Room>? _rooms = [];
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private MonopolyApiClient MonopolyApiClient { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var users = await MonopolyApiClient.GetPlayers();
        _players = users.Select(p => new Player(p.Id, p.Token));
    }

    private async void CreateGame()
    {
        CreateGameBodyPayload bodyPayload = new([.. _players]);
        var host = _players.FirstOrDefault();
        if (host is null)
        {
            return;
        }

        await MonopolyApiClient.CreateGame(host.Token, _players.Select(x => new PlayerModel { Id = x.Id, Token = x.Token }));
        await RefleshRoomListAsync();
    }

    private async Task RefleshRoomListAsync()
    {
        var roomIds = await MonopolyApiClient.GetRooms();
        _rooms = roomIds.Select(id => new Room(id, [.. _players]));
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