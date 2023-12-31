namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerJoinGameEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
}