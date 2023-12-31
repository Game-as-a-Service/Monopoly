namespace SharedLibrary.ResponseArgs.Monopoly;

public class OnlyOnePersonEventArgs : EventArgs
{
    public required string GameStage { get; init; }
}