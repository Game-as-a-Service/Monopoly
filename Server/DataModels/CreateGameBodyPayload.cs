namespace Server.DataModels;

public record CreateGameBodyPayload(Player[] Players);
public record Player(string Id, string Nickname);
