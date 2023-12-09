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
            new(UserId, "AA", false, false),
            new("123", "BB", true, true, ColorEnum.Green),
            new("456", "CC", true, false, ColorEnum.Red, RoleEnum.OldMan),
        ];
    }

    public void Update()
    {
        StateHasChanged();
    }
}