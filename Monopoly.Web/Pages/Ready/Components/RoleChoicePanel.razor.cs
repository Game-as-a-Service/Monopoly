using Client.Pages.Ready.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Ready.Components;

public partial class RoleChoicePanel
{
    [CascadingParameter] public ReadyPage Parent { get; set; } = default!;
    private Player? CurrentPlayer => Parent.CurrentPlayer;
    private void ChangeRole(RoleEnum role)
    {
        if (CurrentPlayer is null)
        {
            return;
        }
        CurrentPlayer.Role = role;
        Parent.Update();
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
