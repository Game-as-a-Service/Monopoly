namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerJoinGameFailedEventArgs : EventArgs
{
    public required string Message { get; init; }
}