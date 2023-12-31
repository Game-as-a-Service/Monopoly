namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerDoesntNeedToPayTollEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required decimal PlayerMoney { get; init; }
}