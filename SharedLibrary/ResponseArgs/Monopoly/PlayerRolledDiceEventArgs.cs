namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerRolledDiceEventArgs : EventArgs
{   
    public required string PlayerId { get; init; }
    public required int DiceCount { get; init; }
}