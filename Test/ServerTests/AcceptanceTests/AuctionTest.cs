using Server.Hubs;
using SharedLibrary;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class AuctionTest
{
    private MonopolyTestServer server = default!;

    [TestInitialize]

    public void SetUp()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 2棟房
                A 持有 2000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 2) * 50% = 1500
        When:   B 喊價 1500
        Then:   B 喊價 成功
        """)]
    public async Task 玩家喊價成功()
    {
        // Arrange
        var A = new { Id = "A", Money = 2000m };
        var B = new { Id = "B", Money = 2000m };
        var A1 = new { Id = "A1", Price = 1000m, HouseCount = 2 };
        var Auction = new { LandId = A1.Id };
        const string gameId = "1";

        var monopolyBuilder = new MonopolyBuilder(gameId)
            .WithPlayer(
                new PlayerBuilder(A.Id)
                    .WithMoney(A.Money)
                    .WithLandContract(A1.Id)
                    .Build()
            )
            .WithPlayer(
                new PlayerBuilder(B.Id)
                    .WithMoney(B.Money)
                    .Build()
            )
            .WithCurrentPlayer(
                new CurrentPlayerStateBuilder(A.Id)
                    .WithAuction(Auction.LandId, null!, 1500)
                    .Build()
            );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBid), gameId, B.Id, 1500);

        // Assert
        // B 喊價
        hub.Verify<string, string, decimal>(
                       nameof(IMonopolyResponses.PlayerBidEvent),
                                (playerId, blockId, highestPrice)
                                => playerId == B.Id && blockId == A1.Id && highestPrice == 1500);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 1棟房
                A 持有 1000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 1) * 50% = 1000
        When:   B 喊價 3000
        Then:   B 喊價 失敗
        """)]
    public async Task 玩家不能喊出比自己現金還高的金額()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 2000m };
        var A1 = new { Id = "A1", Price = 1000m, HouseCount = 1 };
        const string gameId = "1";

        var monopolyBuilder = new MonopolyBuilder(gameId)
            .WithPlayer(
                new PlayerBuilder(A.Id)
                    .WithMoney(A.Money)
                    .WithLandContract(A1.Id)
                    .Build()
            )
            .WithPlayer(
                new PlayerBuilder(B.Id)
                    .WithMoney(B.Money)
                    .Build()
            )
            .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id)
                .WithAuction(A1.Id, null!, 1000)
                .Build()
            );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBid), gameId, "B", 3000);

        // Assert
        // B 喊價
        hub.Verify<string, decimal, decimal, decimal>(
                       nameof(IMonopolyResponses.PlayerTooPoorToBidEvent),
                                (playerId, playerMoney, bidPrice, highestPrice)
                                => playerId == "B" && playerMoney == 2000 && bidPrice == 3000 && highestPrice == 1000);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 1棟房
                A 持有 1000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 1) * 50% = 1000
        When:   B 喊價 800
        Then:   B 喊價 失敗
        """)]
    public async Task 玩家不能喊出比當前拍賣金額更低的金額()
    {
        // Arrange
        var A1 = new { Id = "A1", Price = 1000m, HouseCount = 1 };
        var A = new { Id = "A", Money = 1000m, LandContract = new[] { A1 } };
        var B = new { Id = "B", Money = 2000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithLandContract("A1")
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id)
            .WithAuction(A1.Id, null!, 1000) 
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBid), gameId, "B", 800);

        // Assert
        // B 喊價
        hub.Verify<string, string, decimal, decimal>(
                       nameof(IMonopolyResponses.PlayerBidFailEvent),
                                (playerId, blockId, bidPrice, highestPrice)
                                => playerId == B.Id && blockId == A1.Id && bidPrice == 800 && highestPrice == 1000);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 1棟房
                A 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 1) * 50% = 1000
        When:   A 拍賣房地產 流拍
        Then:   A 持有金額為 2000 + (1000 + 1000 * 1) * 70% = 3400
                A1 為系統所有
        """)]
    public async Task 拍賣結算時流拍系統會以房地產價值的七成收購()
    {
        // Arrange
        var A = new { Id = "A", Money = 2000m };
        var A1 = new { Id = "A1", Price = 1000m, HouseCount = 1 };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithLandContract(A1.Id)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id)
            .WithAuction(A1.Id, null!, 1000)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.EndAuction), gameId, "A");

        // Assert
        // 流拍
        // 在流拍中，土地為系統所有，OwnerMoney值回傳0
        hub.Verify<string, decimal, string, string?, decimal>(
                       nameof(IMonopolyResponses.EndAuctionEvent),
                                (playerId, playerMoney, blockId, owner, ownerMoney)
                                => playerId == "A" && playerMoney == 3400 && blockId == "A1" && owner == null && ownerMoney == 0);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元
                A 持有 1000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 1000 * 50% = 500
                B 喊價 600元
        When:   拍賣結算
        Then:   A 持有金額為 1000 + 600 = 1600
                B 持有金額為 2000 - 600 = 1400
                A1 為 B 所有
        """)]
    public async Task 拍賣結算時轉移金錢及地契()
    {
        // Arrange
        var A1 = new { Id = "A1", Price = 1000m, HouseCount = 1 };
        var A = new { Id = "A", Money = 1000m, LandContract = new[] { A1 } };
        var B = new { Id = "B", Money = 2000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithLandContract(A1.Id)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id)
            .WithAuction(LandId: A1.Id, HighestBidderId: B.Id, HighestPrice: 600)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.EndAuction), gameId, "A");

        // Assert
        // 拍賣結算
        hub.Verify<string, decimal, string, string?, decimal>(
                       nameof(IMonopolyResponses.EndAuctionEvent),
                                (playerId, playerMoney, blockId, owner, ownerMoney)
                                => playerId == "A" && playerMoney == 1600 && blockId == "A1" && owner == "B" && ownerMoney == 1400);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 2棟房
                A 持有 2000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 2) * 50% = 1500
        When:   A 喊價 1500
        Then:   A 喊價 失敗
        """)]
    public async Task 拍賣土地的玩家不能自己喊價()
    {
        // Arrange
        var A1 = new { Id = "A1", Price = 1000m, HouseCount = 2 };
        var A = new { Id = "A", Money = 2000m, LandContract = new[] { A1 } };
        var B = new { Id = "B", Money = 2000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithLandContract(A1.Id)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id)
            .WithAuction(LandId: A1.Id, null, 1500)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBid), gameId, "A", 1500);

        // Assert
        // A 喊價
        hub.Verify<string>(
                       nameof(IMonopolyResponses.CurrentPlayerCannotBidEvent),
                                (playerId)
                                => playerId == "A");
        hub.VerifyNoElseEvent();
    }
}