namespace SharedLibrary.ResponseArgs.ReadyRoom;
public class GetReadyInfoEventArgs : EventArgs
{
    public required IEnumerable<Player> Players { get; init; }
    public required string HostId { get; init; }
    public class Player
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
        public required bool IsReady { get; init; }
        public required ColorEnum Color { get; init; }
        public required RoleEnum Role { get; init; }
    }
    public enum ColorEnum
    {
        None,
        Red,
        Blue,
        Green,
        Yellow,
    }
    public enum RoleEnum
    {
        None,
        OldMan,
        Baby,
        Dai,
        Mei
    }
}
