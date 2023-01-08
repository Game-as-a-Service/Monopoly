using Shared.Domain;

namespace Shared.Usecases.Utils;

public class UsecaseUtils
{
    public static void GameSetup()
    {
        var map = new Map(Shared.Utils.SevenXSevenMap());
        var game = new Game("g1", map);
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
    }

    public static Game GetGameById(string id)
    {
        var gameRepository = new JsonRepository();
        return gameRepository.FindGameById(id);
    }

    public static void SetGameDice(string id, int dice)
    {
        var gameRepository = new JsonRepository();
        var game = gameRepository.FindGameById(id);
        game.CurrentDice = dice;
        gameRepository.Save(game);
    }
}