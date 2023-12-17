namespace SharedLibrary.ResponseArgs;
public class GetReadyInfoEvent : EventArgs
{
    public required IEnumerable<Player> Players { get; set; }
    public required string HostId { get; set; }
    public class Player
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsReady { get; set; } = false;
        public ColorEnum Color { get; set; } = ColorEnum.None;
        public RoleEnum Role { get; set; } = RoleEnum.None;
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
