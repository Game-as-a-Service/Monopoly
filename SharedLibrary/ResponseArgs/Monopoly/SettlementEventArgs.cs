namespace SharedLibrary.ResponseArgs.Monopoly;

public class SettlementEventArgs : EventArgs
{
    public required int Rounds { get; init; }
    public required string[] PlayerIds { get; init; }
}