namespace SharedLibrary.ResponseArgs;

public class PlayerBankruptEvent : EventArgs
{
    public required string PlayerId { get; set; }
}