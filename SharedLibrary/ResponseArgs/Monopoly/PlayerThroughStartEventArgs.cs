namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerThroughStartEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required decimal GainMoney { get; init; }
    public required decimal TotalMoney { get; init; }
}