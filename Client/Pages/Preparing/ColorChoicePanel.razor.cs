using Client.Pages.Preparing.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Preparing;

public partial class ColorChoicePanel
{
    [CascadingParameter] public IList<Player> Players { get; set; } = default!;
    [CascadingParameter] public string UserId { get; set; } = string.Empty;
    [CascadingParameter] public PreparingPage Parent { get; set; } = default!;
    private Player CurrentPlayer => Players.First(p => p.Id == UserId);
    private string ColorSelected(ColorEnum color) => CurrentPlayer.Color == color ? "selected" : "";
    private string RoleSelected(ColorEnum color)
    {
        var role = GetPlayerWithColor(color)?.Role;
        if (role is not null)
        {
            return role != RoleEnum.None ? "role-selected" : "";
        }
        return "";
    }

    private void ChangeColor(ColorEnum color)
    {
        var player = Players.FirstOrDefault(p => p.Id == UserId);
        if (player is not null)
        {
            Players.Remove(player);
            Players.Add(player with { Color = color });
            Parent.Update();
        }
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