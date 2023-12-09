namespace Client.Pages.Preparing.Entities;

public class Player(string Id, string Name, bool IsHost, bool IsReady, ColorEnum Color = ColorEnum.None, RoleEnum Role = RoleEnum.None)
{
    public string Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public bool IsHost { get; set; } = IsHost;
    public bool IsReady { get; set; } = IsReady;
    public ColorEnum Color { get; set; } = Color;
    public RoleEnum Role { get; set; } = Role;
}
