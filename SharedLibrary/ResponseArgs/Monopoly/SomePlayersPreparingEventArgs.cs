namespace SharedLibrary.ResponseArgs.Monopoly;

public class SomePlayersPreparingEventArgs : EventArgs
{
    public required string GameStage { get; init; }
    public required string[] PlayerIds { get; init; }
}