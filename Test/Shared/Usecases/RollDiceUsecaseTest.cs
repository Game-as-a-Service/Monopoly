using Shared.Domain;
using Shared.Usecases.Utils;
namespace Shared.Usecases;

[TestClass]
public class RollDiceUsecaseTest
{

    [TestMethod]
    public void 輪到玩家A_玩家A在起點__玩家擲骰子__沒有錯誤()
    {
        // Arrange
        const string GameId = "g1";

        UsecaseUtils.GameSetup();
        RollDiceUsecase.Input input = new(GameId, "p1");
        var presenter = new RollDiceUsecase.Presenter();

        // Act
        var rollDiceUsecase = new RollDiceUsecase(new JsonRepository());
        rollDiceUsecase.Execute(input, presenter);

        // Assert
    }
}