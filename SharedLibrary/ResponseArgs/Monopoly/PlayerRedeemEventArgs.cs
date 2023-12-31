namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerRedeemEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required decimal PlayerMoney { get; init; }
    public required string LandId { get; init; }
}