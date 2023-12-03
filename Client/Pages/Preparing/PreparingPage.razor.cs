using Client.Pages.Preparing.Entities;

namespace Client.Pages.Preparing;

public partial class PreparingPage
{
    IList<Player> Players { get; set; } = default!;
    string UserId { get; set; } = string.Empty;

    override protected void OnInitialized()
    {
        base.OnInitialized();
        Players = new List<Player>() { new(UserId, "AA", false, false) };
    }

    public void Update()
    {
        StateHasChanged();
    }
}