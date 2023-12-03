namespace Client.Pages.Preparing.Entities;

public record Player(string Id, string Name, bool IsHost, bool IsReady, ColorEnum Color = ColorEnum.None, RoleEnum Role = RoleEnum.None);
