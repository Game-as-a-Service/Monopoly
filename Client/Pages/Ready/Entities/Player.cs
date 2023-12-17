namespace Client.Pages.Ready.Entities;

public class Player
{
    private ColorEnum color = ColorEnum.None;

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsHost { get; set; } = false;
    public bool IsReady { get; set; } = false;
    public ColorEnum Color
    {
        get => color; set
        {
            color = value;
        }
    }
    public RoleEnum Role { get; set; } = RoleEnum.None;
}
