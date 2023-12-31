using Server.Hubs;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
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
        var A = new { Id = "A", Money = 5000m };
        var F4 = new { Id = "F4", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder(gameId)
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition(F4.Id, Direction.Up)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act

        await hub.SendAsync(nameof(MonopolyHub.PlayerBuyLand), gameId, "A", "F4");

        // Assert
        // A 購買土地
        hub.Verify(nameof(IMonopolyResponses.PlayerBuyBlockEvent),
                  (PlayerBuyBlockEventArgs e) => e is { PlayerId: "A", LandId: "F4" });

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
        var A = new { Id = "A", Money = 500m };
        var F4 = new { Id = "F4", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition(F4.Id, Direction.Up)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act

        await hub.SendAsync(nameof(MonopolyHub.PlayerBuyLand), gameId, "A", "F4");

        // Assert
        // A 購買土地金額不足
        hub.Verify(nameof(IMonopolyResponses.PlayerBuyBlockInsufficientFundsEvent),
                  (PlayerBuyBlockInsufficientFundsEventArgs e) => e is { PlayerId: "A", LandId: "F4", Price: 1000 });

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
        var A = new { Id = "A", Money = 5000m };
        var B = new { Id = "B", Money = 5000m };
        var F4 = new { Id = "F4", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition(F4.Id, Direction.Right)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithLandContract(F4.Id)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act

        await hub.SendAsync(nameof(MonopolyHub.PlayerBuyLand), gameId, "A", "F4");

        // Assert
        // A 購買土地非空地
        hub.Verify(nameof(IMonopolyResponses.PlayerBuyBlockOccupiedByOtherPlayerEvent),
                   (PlayerBuyBlockOccupiedByOtherPlayerEventArgs e) => e is { PlayerId: "A", LandId: "F4" });

        hub.VerifyNoElseEvent();
    }

    // TODO: 目前可以指定要買哪個地，但應該只能買腳下的土地，玩家會發送 BuyLand 而不是 BuyLand(A1)
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
    [Ignore]
    public void 玩家無法購買非腳下的土地()
    {
        // Arrange
        //var A = new { Id = "A", Money = 5000m };


        //const string gameId = "1";
        //var monopolyBuilder = new MonopolyBuilder("1")
        //.WithPlayer(
        //    new PlayerBuilder(A.Id)
        //    .WithMoney(A.Money)
        //    .WithPosition("F2", Direction.Up.ToString())
        //)
        //.WithCurrentPlayer(nameof(A));

        //monopolyBuilder.Save(server);

        //var hub = await server.CreateHubConnectionAsync(gameId, "A");

        //// Act

        //await hub.SendAsync(nameof(MonopolyHub.PlayerBuyLand), gameId, "A", "F4");

        //// Assert
        //// A 購買土地非腳下的土地
        //hub.Verify<string, string>(
        //               nameof(IMonopolyResponses.PlayerBuyBlockMissedLandEvent),
        //                          (playerId, blockId) => playerId == "A" && blockId == "F4");

        //hub.VerifyNoElseEvent();
    }
}