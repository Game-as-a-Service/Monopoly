namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerCannotReadyEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string PlayerState { get; init; }
    public required string? RoleId { get; init; }
    public required int LocationId { get; init; }
}