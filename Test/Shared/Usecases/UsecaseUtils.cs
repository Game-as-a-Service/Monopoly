using Server.Repositories;
using Shared.Domain;
using Shared.Domain.Interfaces;

namespace SharedTests.Usecases;

public class UsecaseUtils
{
    public static Game GameSetup(IDice[]? dice = null)
    {
        var map = new Map(Utils.SevenXSevenMap());
        var game = new Game("g1", map, dice);
        var playerA = new Player("p1");
        var playerB = new Player("p2");
        var playerC = new Player("p3");
        var playerD = new Player("p4");

        game.AddPlayer(playerA);
        game.AddPlayer(playerB);
        game.AddPlayer(playerC);
        game.AddPlayer(playerD);

        game.Initial();

        var gameRepository = new InMemoryRepository();
        gameRepository.Save(game);

        return game;
    }

    public static Game GetGameById(string id)
    {
        var gameRepository = new InMemoryRepository();
        return gameRepository.FindGameById(id);
    }
}