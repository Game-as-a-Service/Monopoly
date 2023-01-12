using Shared.Domain;

namespace Shared.Usecases.Utils;

public class UsecaseUtils
{
    public static Game GameSetup(DiceSetting? diceSetting = null)
    {
        var map = new Map(Shared.Utils.SevenXSevenMap());
        var game = new Game("g1", map, diceSetting);
        var playerA = new Player("p1");
        var playerB = new Player("p2");
        var playerC = new Player("p3");
        var playerD = new Player("p4");
        
        game.AddPlayer(playerA);
        game.AddPlayer(playerB);
        game.AddPlayer(playerC);
        game.AddPlayer(playerD);

        game.Initial();

        var gameRepository = new JsonRepository();
        gameRepository.Save(game);

        return game;
    }

    public static Game GetGameById(string id)
    {
        var gameRepository = new JsonRepository();
        return gameRepository.FindGameById(id);
    }
}