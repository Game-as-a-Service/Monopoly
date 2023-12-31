namespace SharedLibrary.ResponseArgs.Monopoly;

public class CannotGetRewardBecauseStandOnStartEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
}