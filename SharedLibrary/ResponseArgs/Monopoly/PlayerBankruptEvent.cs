namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerBankruptEvent : EventArgs
{
    public required string PlayerId { get; init; }
}