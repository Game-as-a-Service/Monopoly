using Client.Pages.Preparing.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Preparing;

public partial class ReadyButton
{
    [CascadingParameter] public PreparingPage Parent { get; set; } = default!;
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
