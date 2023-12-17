namespace SharedLibrary.ResponseArgs;
public class WelcomeEvent : EventArgs
{
    public required string PlayerId { get; set; }
}
