using Client.Pages.Ready.Entities;

namespace Client.Pages.Ready;

public partial class ReadyPage
{
    public IEnumerable<Player> Players { get; set; } = [];
    public string UserId { get; set; } = string.Empty;
    public Player? CurrentPlayer => Players.FirstOrDefault(x => x.Id == UserId);

    public void Update()
    {
        StateHasChanged();
    }
}