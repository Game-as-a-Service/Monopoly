using Shared.Domain;
using Shared.Repositories;

namespace Server.Repositories;

public class InMemoryRepository : IRepository
{
    private static readonly Dictionary<string, Game> Games = new();

    public Game FindGameById(string id)
    {
        Games.TryGetValue(id, out Game? game);
        return game;
    }

    public void Save(Game game)
    {
        Games[game.Id] = game;
    }
}