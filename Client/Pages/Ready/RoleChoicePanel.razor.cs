using Client.Pages.Ready.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Ready;

public partial class RoleChoicePanel
{
    [CascadingParameter] public ReadyPage Parent { get; set; } = default!;
    IEnumerable<Player> Players => Parent.Players;
    string UserId => Parent.UserId;
    private Player CurrentPlayer => Parent.CurrentPlayer;
    private void ChangeRole(RoleEnum role)
    {
        var player = Players.FirstOrDefault(p => p.Id == UserId);
        if (player is not null)
        {
            CurrentPlayer.Role = role;
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
