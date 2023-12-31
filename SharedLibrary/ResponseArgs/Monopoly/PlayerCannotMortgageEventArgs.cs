namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerCannotMortgageEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
    public required decimal PlayerMoney { get; init; }
}