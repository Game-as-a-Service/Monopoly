using Client.Pages.Ready.Entities;

namespace Client.Pages.Ready;

public partial class ReadyPage
{
    public IEnumerable<Player> Players { get; set; } = default!;
    public string UserId { get; set; } = string.Empty;
    public Player CurrentPlayer => Players.First(x => x.Id == UserId);

    override protected void OnInitialized()
    {
        Players = [
            new Player { Id = UserId, Name = "AA", IsHost = true, IsReady = false },
            new Player { Id = "123", Name = "BB", IsHost = false, IsReady = true, Color = ColorEnum.Red },
            new Player { Id = "456", Name = "CC", IsHost = false, IsReady = true, Color = ColorEnum.Green, Role = RoleEnum.Dai }
        ];
    }

    public void Update()
    {
        StateHasChanged();
    }
}