namespace SharedLibrary.ResponseArgs.Monopoly;

public class MortgageDueEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
}