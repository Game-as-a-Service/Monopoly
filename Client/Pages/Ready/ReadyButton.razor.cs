using Client.Pages.Ready.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Ready;

public partial class ReadyButton
{
    [CascadingParameter] public ReadyPage Parent { get; set; } = default!;
    Player CurrentPlayer => Parent.CurrentPlayer;
    private void Ready()
    {
        CurrentPlayer.IsReady = !CurrentPlayer.IsReady;
        Parent.Update();
    }
    private string GetReadyButtonCss()
    {
        return CurrentPlayer.IsReady ? "selected" : string.Empty;
    }
}
