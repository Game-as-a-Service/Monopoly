using System.Text.Json;
using Shared.Domain;
using Shared.Repositories;
using static Shared.Domain.Map;

namespace Shared.Usecases.Utils;

public class JsonRepository : IRepository
{
    public Game FindGameById(string id)
    {
        string stringGame = File.ReadAllText($"./{id}.json");
        var jsonGame = JsonSerializer.Deserialize<JsonGame>(stringGame);

        var map = new Map(Shared.Utils.SevenXSevenMap());
        Game game = new(jsonGame.Id, map);
        foreach (var jsonPlayer in jsonGame.Players)
        {
            game.AddPlayer(new Player(jsonPlayer.Id));
            game.SetPlayerToBlock(game.Players.Last(), jsonPlayer.CurrentBlockId, (Direction)Enum.Parse(typeof(Direction), jsonPlayer.CurrentDirection));
        }
        game.CurrentPlayer = jsonGame.CurrentPlayer == null ? null : game.Players.Where(p => p.Id == jsonGame.CurrentPlayer.Id).First();
        game.CurrentDice = jsonGame.CurrentDice;
        return game;
    }

    public void Save(Game game)
    {
        JsonGame jsonGame = new()
        {
            Id = game.Id,
            CurrentDice = game.CurrentDice,
            CurrentPlayer = new JsonPlayer
            {
                Id = game.CurrentPlayer.Id,
                CurrentBlockId = game.PlayerPositionDictionary.Where(p => p.Key.Id == game.CurrentPlayer.Id).FirstOrDefault().Value.block.Id,
                CurrentDirection = game.PlayerPositionDictionary.Where(p => p.Key.Id == game.CurrentPlayer.Id).FirstOrDefault().Value.direction.ToString()
            },
            Players = game.Players.Select(p => new JsonPlayer
            {
                Id = p.Id,
                CurrentBlockId = game.PlayerPositionDictionary.Where(pp => pp.Key.Id == p.Id).FirstOrDefault().Value.block.Id,
                CurrentDirection = game.PlayerPositionDictionary.Where(pp => pp.Key.Id == p.Id).FirstOrDefault().Value.direction.ToString()
            }).ToList()
        };
        string stringGame = JsonSerializer.Serialize(jsonGame);
        File.WriteAllText($"./{game.Id}.json", stringGame);
    }

    private class JsonGame
    {
        public string Id { get; set; }
        public int CurrentDice { get; set; }
        public JsonPlayer CurrentPlayer { get; set; }
        public List<JsonPlayer> Players { get; set; }
    }

    private class JsonPlayer
    {
        public string Id { get; set; }
        public string CurrentBlockId { get; set; }
        public string CurrentDirection { get; set; }
    }
}

