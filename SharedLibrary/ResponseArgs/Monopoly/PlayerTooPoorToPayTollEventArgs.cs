namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerTooPoorToPayTollEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required decimal PlayerMoney { get; init; }
    public required decimal Toll { get; init; }
}