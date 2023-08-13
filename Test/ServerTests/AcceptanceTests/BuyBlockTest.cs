using Domain;
using Server.Hubs;
using SharedLibrary;
using static Domain.Map;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class BuyBlockTest
{
    private MonopolyTestServer server = default!;

    [TestInitialize]
    public void Setup()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A資產5000元、沒有房地產、目前輪到A、Ａ在F4上
                F4是空地、購買價1000元
        When:   玩家A購買土地
        Then:   玩家A持有金額為4000
                玩家A持有房地產數量為1
                玩家A持有房地產為F4
        """)]
    public async Task 玩家在空地上可以購買土地()
    {
        // Arrange
        Player A = new("A", 5000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder(gameId)
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("F4", Direction.Up.ToString())
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act

        await hub.SendAsync(nameof(MonopolyHub.PlayerBuyLand), gameId, "A", "F4");

        // Assert
        // A 購買土地
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.PlayerBuyBlockEvent),
                                  (playerId, blockId) => playerId == "A" && blockId == "F4");

        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A資產500元,沒有房地產
                目前輪到A,A在F4上
                F4是空地,購買價1000元
        When:   玩家A購買土地
        Then:   顯示錯誤訊息"金額不足"
                玩家A持有金額為500
                玩家A持有房地產數量為0
        """)]
    public async Task 金錢不夠無法購買土地()
    {
        // Arrange
        Player A = new("A", 500);
        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("F4", Direction.Up.ToString())
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act

        await hub.SendAsync(nameof(MonopolyHub.PlayerBuyLand), gameId, "A", "F4");

        // Assert
        // A 購買土地金額不足
        hub.Verify<string, string, decimal>(
                       nameof(IMonopolyResponses.PlayerBuyBlockInsufficientFundsEvent),
                                  (playerId, blockId, landMoney) => playerId == "A" && blockId == "F4" && landMoney == 1000);

        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A資產5000元、沒有房地產
                玩家B資產5000元、擁有房地產F4
                目前輪到A、Ａ在F4上
        When:   玩家A購買土地F4
        Then:   顯示錯誤訊息“無法購買”
                玩家A持有金額為5000
                玩家A持有房地產數量為0
                玩家B持有金額為5000
                玩家B持有房地產數量為1，持有F4
        """)]
    public async Task 玩家在有地主的土地上不可以購買土地()
    {
        // Arrange
        Player A = new("A", 5000);
        Player B = new("B", 5000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("F4", Direction.Up.ToString())
        )
        .WithPlayer(
            new MonopolyPlayer(B.Id)
            .WithMoney(B.Money)
            .WithPosition("F4", Direction.Up.ToString())
            .WithLandContract("F4")
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act

        await hub.SendAsync(nameof(MonopolyHub.PlayerBuyLand), gameId, "A", "F4");

        // Assert
        // A 購買土地非空地
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.PlayerBuyBlockOccupiedByOtherPlayerEvent),
                                  (playerId, blockId) => playerId == "A" && blockId == "F4");

        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A資產5000元，沒有房地產
                目前輪到A，A在F2上
        When:   玩家A購買土地F4
        Then:   顯示錯誤訊息"必須在購買的土地上才可以購買"
                玩家A持有金額為5000
                玩家A持有房地產數量為0
        """)]
    public async Task 玩家無法購買非腳下的土地()
    {
        // Arrange
        Player A = new("A", 5000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("F2", Direction.Up.ToString())
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act

        await hub.SendAsync(nameof(MonopolyHub.PlayerBuyLand), gameId, "A", "F4");

        // Assert
        // A 購買土地非腳下的土地
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.PlayerBuyBlockMissedLandEvent),
                                  (playerId, blockId) => playerId == "A" && blockId == "F4");

        hub.VerifyNoElseEvent();
    }
}