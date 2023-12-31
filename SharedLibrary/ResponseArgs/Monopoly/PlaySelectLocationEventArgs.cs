namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlaySelectLocationEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required int LocationId { get; init; }
}