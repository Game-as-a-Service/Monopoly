namespace SharedLibrary.ResponseArgs.Monopoly;

public class GameStartEventArgs : EventArgs
{
    public required string GameStage { get; init; }
    public required string CurrentPlayerId { get; init; }
}