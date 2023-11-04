using Domain.Builders;
using Domain.Events;

namespace DomainTests.Testcases;

[TestClass]

public class GameStartTest
{
    [TestMethod]
    [Description(
        """
        Given:  房主為 A, 已準備
                玩家 B, 已準備
        When:   A 開始遊戲
        Then:   遊戲開始
        DomainEvent: 遊戲開始, 為 A 的回合
        """)]
    public void 房主成功開始遊戲()
    {
        // Arrange
        var A = new { Id = "A", locationId = 1, roleId = "1", preparedState = PlayerState.Normal };
        var B = new { Id = "B", locationId = 2, roleId = "2", preparedState = PlayerState.Normal };
        var GameStart = new { gameState = "Gaming", currentPlayer = "A" };

        var monopoly = new MonopolyBuilder()
            .WithPlayer(A.Id, pa => pa.WithLocation(A.locationId).WithRole(A.roleId).WithState(A.preparedState))
            .WithPlayer(B.Id, pb => pb.WithLocation(B.locationId).WithRole(B.roleId).WithState(B.preparedState))
            .WithGameStage(GameStage.Preparing)
            .WithHost(A.Id)
            .Build();

        // Act 
        monopoly.GameStart(A.Id);

        // Assert
        Assert.AreEqual(GameStart.gameState, monopoly.GameStage.ToString());
        Assert.AreEqual(GameStart.currentPlayer, monopoly.CurrentPlayerState.PlayerId);
        // A
        Assert.AreEqual("A" ,monopoly.Players.First(p => p.Id == A.Id).Id);
        Assert.AreEqual(15000, monopoly.Players.First(p => p.Id == A.Id).Money);
        Assert.AreEqual("Normal", monopoly.Players.First(p => p.Id == A.Id).State.ToString());
        Assert.AreEqual(0, monopoly.Players.First(p => p.Id == A.Id).BankruptRounds);
        Assert.AreEqual("Start", monopoly.Players.First(p => p.Id == A.Id).Chess.CurrentBlockId);
        // B
        Assert.AreEqual("B" ,monopoly.Players.First(p => p.Id == B.Id).Id);
        Assert.AreEqual(15000, monopoly.Players.First(p => p.Id == B.Id).Money);
        Assert.AreEqual("Normal", monopoly.Players.First(p => p.Id == B.Id).State.ToString());
        Assert.AreEqual(0, monopoly.Players.First(p => p.Id == B.Id).BankruptRounds);
        Assert.AreEqual("Start", monopoly.Players.First(p => p.Id == B.Id).Chess.CurrentBlockId);

        monopoly.DomainEvents.NextShouldBe(new GameStartEvent(GameStart.gameState, GameStart.currentPlayer));
    }

    [TestMethod]
    [Description(
        """
        Given:  房主為 A, 已準備
                沒有其他玩家
        When:   A 開始遊戲
        Then:   開始遊戲失敗
        DomainEvent: 只有一人, 開始遊戲失敗
        """)]
    public void 人數只有1人_房主開始遊戲失敗()
    {
        // Arrange
        var A = new { Id = "A", locationId = 1, roleId = "1", preparedState = PlayerState.Normal };
        var GameState = "Preparing";

        var monopoly = new MonopolyBuilder()
            .WithPlayer(A.Id, pa => pa.WithLocation(A.locationId).WithRole(A.roleId).WithState(A.preparedState))
            .WithGameStage(GameStage.Preparing)
            .WithHost(A.Id)
            .Build();

        // Act 
        monopoly.GameStart(A.Id);

        // Assert
        Assert.AreEqual(GameState, monopoly.GameStage.ToString());

        monopoly.DomainEvents.NextShouldBe(new OnlyOnePersonEvent("Preparing"));
    }

    [TestMethod]
    [Description(
        """
        Given:  房主為 A
                A: 位置1, 角色A, 已準備
                B: 未準備
        When:   A 開始遊戲
        Then:   開始遊戲失敗
        DomainEvent: 尚有玩家準備中, 開始遊戲失敗
        """)]
    public void 有人沒有準備_房主開始遊戲失敗()
    {
        // Arrange
        var A = new { Id = "A", roleId = "A", locationId = 1, preparedState = PlayerState.Normal };
        var B = new { Id = "B", roleId = "B", locationId = 0, preparingState = PlayerState.Preparing };
        var GameState = "Preparing";

        var monopoly = new MonopolyBuilder()
            .WithPlayer(A.Id, pa => pa.WithLocation(A.locationId)
                                        .WithState(A.preparedState)
                                        .WithRole(A.roleId)
                                        )
            .WithPlayer(B.Id, pb => pb.WithLocation(B.locationId)
                                        .WithState(B.preparingState)
                                        .WithRole(B.roleId)
                                        )
            .WithGameStage(GameStage.Preparing)
            .WithHost(A.Id)
            .Build();

        // Act 
        monopoly.GameStart(A.Id);

        // Assert
        Assert.AreEqual(GameState, monopoly.GameStage.ToString());

        monopoly.DomainEvents.NextShouldBe(new SomePlayersPreparingEvent(GameState, "B"));
    }
}