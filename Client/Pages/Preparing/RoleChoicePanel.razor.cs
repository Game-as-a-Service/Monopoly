using Client.Pages.Preparing.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Preparing;

public partial class RoleChoicePanel
{
    [CascadingParameter] public IList<Player> Players { get; set; } = default!;
    [CascadingParameter] public string UserId { get; set; } = string.Empty;
    [CascadingParameter] public PreparingPage Parent { get; set; } = default!;
    private Player CurrentPlayer => Players.First(p => p.Id == UserId);
    private void ChangeRole(RoleEnum role)
    {
        var player = Players.FirstOrDefault(p => p.Id == UserId);
        if (player is not null)
        {
            Players.Remove(player);
            Players.Add(player with { Role = role });
            Parent.Update();
        }
    }
}

public static class RoleEnumExtensions
{
    public static string ToLowerCaseName(this RoleEnum role)
    {
        return role switch
        {
            RoleEnum.None => "none",
            RoleEnum.OldMan => "oldman",
            RoleEnum.Baby => "baby",
            RoleEnum.Dai => "dai",
            RoleEnum.Mei => "mei",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}
