using Application.Common;
using Domain;

namespace ApplicationTests;

public class Utils
{
    public class InMemoryRepository : IRepository
    {
        private static readonly Dictionary<string, Monopoly> Games = new();

        public Monopoly FindGameById(string id)
        {
            Games.TryGetValue(id, out Monopoly? game);
            return game;
        }

        public string Save(Monopoly game)
        {
            Games[game.Id] = game;
            return game.Id;
        }
    }
}