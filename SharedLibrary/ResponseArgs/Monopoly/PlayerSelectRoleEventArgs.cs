namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerSelectRoleEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string RoleId { get; init; }
}