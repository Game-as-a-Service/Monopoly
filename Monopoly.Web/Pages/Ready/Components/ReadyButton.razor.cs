using Client.Pages.Ready.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Ready.Components;

public partial class ReadyButton
{
    [CascadingParameter] public ReadyPage Parent { get; set; } = default!;
    Player? CurrentPlayer => Parent.CurrentPlayer;
    bool EnabledToReady => CurrentPlayer?.Color is not ColorEnum.None && CurrentPlayer?.Role is not RoleEnum.None;
    private void Ready()
    {
        if (!EnabledToReady || CurrentPlayer is null)
            return;
        CurrentPlayer.IsReady = !CurrentPlayer.IsReady;
        Parent.Update();
    }
    private static void Start()
    {

    }
    private string GetReadyButtonCss()
    {
        if (CurrentPlayer is null)
            return string.Empty;
        var enabledToReadyCss = EnabledToReady ? "enabled-to-ready" : string.Empty;
        return CurrentPlayer.IsReady ? "selected" : enabledToReadyCss;
    }
    private string GetStartButtonCss()
    {
        var enabledToStart = EnabledToReady && Parent.Players.Where(p => p != CurrentPlayer).All(p => p.IsReady);
        return enabledToStart ? "enabled-to-start" : string.Empty;
    }
}
