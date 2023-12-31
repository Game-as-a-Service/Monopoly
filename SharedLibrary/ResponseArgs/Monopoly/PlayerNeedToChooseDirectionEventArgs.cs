namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerNeedToChooseDirectionEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required string[] Directions { get; init; }
}