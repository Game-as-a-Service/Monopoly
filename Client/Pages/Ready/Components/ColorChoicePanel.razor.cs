using Client.Pages.Ready.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Ready.Components;

public partial class ColorChoicePanel
{
    [CascadingParameter] public ReadyPage Parent { get; set; } = default!;
    IEnumerable<Player> Players => Parent.Players;
    private Player? CurrentPlayer => Parent.CurrentPlayer;

    private void ChangeColor(ColorEnum color)
    {
        if (GetPlayerWithColor(color) is not null || CurrentPlayer is null)
        {
            return;
        }
        CurrentPlayer.Color = color;
        Parent.Update();
    }

    private string GetChoiceWrapperCss(ColorEnum color)
    {
        var player = GetPlayerWithColor(color);
        if (player is null)
        {
            return string.Empty;
        }
        return $"color-selected {(color == CurrentPlayer?.Color ? "current-player" : string.Empty)}";
    }

    private static string GetReadySignCss(Player? player)
    {
        if (player is null)
        {
            return string.Empty;
        }
        return player.IsReady ? "ready" : string.Empty;
    }

    private Player? GetPlayerWithColor(ColorEnum color)
    {
        return Players.FirstOrDefault(p => p.Color == color);
    }
}

public static class ColorEnumExtensions
{
    public static string ToLowerCaseName(this ColorEnum color)
    {
        return color switch
        {
            ColorEnum.None => "none",
            ColorEnum.Red => "red",
            ColorEnum.Blue => "blue",
            ColorEnum.Green => "green",
            ColorEnum.Yellow => "yellow",
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    }
}