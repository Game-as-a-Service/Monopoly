using Shared.Usecases.Utils;

namespace Shared.Usecases;

[TestClass]
public class MoveChessUsecaseTest
{
    [TestMethod]
    public void 輪到A_A在起點_擲骰子得到5__移動棋子__棋子會在A4()
    {
        // Arrange
        const string GameId = "g1";
        const string PlayerId = "p1";

        UsecaseUtils.GameSetup();
        UsecaseUtils.SetGameDice(GameId, 5);
        MoveChessUsecase.Input input = new(GameId, PlayerId);
        var presenter = new MoveChessUsecase.Presenter();

        // Act
        var moveChessUsecase = new MoveChessUsecase(new JsonRepository());
        moveChessUsecase.Execute(input, presenter);

        // Assert
        Assert.AreEqual("A4", presenter.ChessPosition);
    }
}