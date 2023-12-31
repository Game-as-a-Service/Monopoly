namespace SharedLibrary;

public class PlayerNeedsToPayTollEventArgs
{
    public required string PlayerId { get; init; }
    public required string OwnerId { get; init; }
    public required decimal Toll { get; init; }
}