using Server.Repositories;
using Shared.Usecases;
using SharedTests;

namespace SharedTests.Usecases;

[TestClass]
public class RollDiceUsecaseTest
{

    [TestMethod]
    [Description(
        """
        Given:  輪到玩家A
                玩家A在起點
        When:   玩家擲骰子
        Then:   不需要選擇方向
        """)]
    public void 玩家擲骰子後不需要選擇方向()
    {
        // Arrange
        const string GameId = "g1";

        UsecaseUtils.GameSetup(Utils.MockDice(2, 3));
        RollDiceUsecase.Input input = new(GameId, "p1");
        var presenter = new RollDiceUsecase.Presenter();

        // Act
        var rollDiceUsecase = new RollDiceUsecase(new InMemoryRepository());
        rollDiceUsecase.Execute(input, presenter);

        // Assert
    }
}