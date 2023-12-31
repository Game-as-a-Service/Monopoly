namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerPayTollEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required decimal PlayerMoney { get; init; }
    public required string OwnerId { get; init; }
    public required decimal OwnerMoney { get; init; }
}