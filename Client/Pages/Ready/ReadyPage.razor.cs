using Client.Pages.Ready.Entities;

namespace Client.Pages.Ready;

public partial class ReadyPage
{
    public IEnumerable<Player> Players { get; set; } = default!;
    public string UserId { get; set; } = string.Empty;
    public Player CurrentPlayer => Players.First(x => x.Id == UserId);

    override protected void OnInitialized()
    {

    }

    public void Update()
    {
        StateHasChanged();
    }
}