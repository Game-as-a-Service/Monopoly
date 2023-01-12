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

        if (jsonGame == null)
        {
            throw new FormatException($"Invalid Json Game (id:{id})");
        }

        var map = new Map(Shared.Utils.SevenXSevenMap());
        Game game = new(jsonGame.Id, map, new DiceSetting
        {
            NumberOfDice = jsonGame.DiceSetting.NumberOfDice,
            Min = jsonGame.DiceSetting.Min, 
            Max = jsonGame.DiceSetting.Max,
        });

        foreach (var jsonPlayer in jsonGame.Players)
        {
            var player = new Player(jsonPlayer.Id);
            game.AddPlayer(player);
            game.SetPlayerToBlock(player, jsonPlayer.CurrentBlockId, (Direction)Enum.Parse(typeof(Direction), jsonPlayer.CurrentDirection));
        }
        game.CurrentPlayer = game.Players.Where(p => p.Id == jsonGame.CurrentPlayerId).FirstOrDefault();
        game.CurrentDice = jsonGame.CurrentDice;

        return game;
    }

    public void Save(Game game)
    {
        JsonGame jsonGame = new()
        { 
            Id = game.Id,
            CurrentDice = game.CurrentDice,
            CurrentPlayerId = game.CurrentPlayer?.Id ?? string.Empty,
            DiceSetting = new JsonDiceSetting
            {
                Max = game.DiceSetting.Max,
                Min  = game.DiceSetting.Min,
                NumberOfDice = game.DiceSetting.NumberOfDice,
            },
            Players = game.Players.Select(p => new JsonPlayer
            {
                Id = p.Id,
                CurrentBlockId = game.PlayerPositionDictionary.Where(pp => pp.Key.Id == p.Id).FirstOrDefault().Value.block.Id,
                CurrentDirection = game.PlayerPositionDictionary.Where(pp => pp.Key.Id == p.Id).FirstOrDefault().Value.direction.ToString()
            }).ToList(),
        };
        string stringGame = JsonSerializer.Serialize(jsonGame);
        File.WriteAllText($"./{game.Id}.json", stringGame);
    }

    private class JsonGame
    {
        public required string Id { get; set; }
        public int[]? CurrentDice { get; set; }
        public required string CurrentPlayerId { get; set; }
        public required JsonDiceSetting DiceSetting { get; set; }

        public List<JsonPlayer> Players { get; set; } = new();
    }

    private class JsonPlayer
    {
        public required string Id { get; set; }
        public required string CurrentBlockId { get; set; }
        public required string CurrentDirection { get; set; }
    }

    private class JsonDiceSetting
    {
        public int Max { get; set; }
        public int Min { get; set; }
        public int NumberOfDice { get; set; }
    }
}

