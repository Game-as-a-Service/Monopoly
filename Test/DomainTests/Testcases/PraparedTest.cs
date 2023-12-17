using Domain.Builders;
using Domain.Events;

namespace DomainTests.Testcases;

[TestClass]

public class PreParedTest
{
    [TestMethod]
    [Description(
        """
        Given:  房主為 A
                A: 位置1, 角色A
                B: 位置2, 角色B
        When:   B 按下準備
        Then:   B 的準備狀態:已準備
        DomainEvent: B 的準備狀態:已準備
        """)]
    public void 玩家成功準備()
    {
        // Arrange
        var A = new { Id = "A", locationId = 1 };
        var B = new { Id = "B", locationId = 2, roleId = "1", preparedState = PlayerState.Normal };

        var monopoly = new MonopolyBuilder()
            .WithPlayer(A.Id, pa => pa.WithLocation(A.locationId).WithState(PlayerState.Ready))
            .WithPlayer(B.Id, pb => pb.WithLocation(B.locationId).WithRole(B.roleId).WithState(PlayerState.Ready))
            .WithGameStage(GameStage.Preparing)
            .Build();

        // Act 
        monopoly.PlayerPrepare(B.Id);

        // Assert
        Assert.AreEqual(B.preparedState, monopoly.Players.First(p => p.Id == B.Id).State);

        monopoly.DomainEvents.NextShouldBe(new PlayerReadyEvent(B.Id, B.preparedState.ToString()));
    }

    [TestMethod]
    [Description(
        """
        Given:  房主為 A
                A: 位置1, 角色A, 已準備
        When:   A 取消準備
        Then:   A 的準備狀態:準備中
        DomainEvent: A 的準備狀態:準備中
        """)]
    public void 玩家取消準備()
    {
        // Arrange
        var A = new { Id = "A", locationId = 1, roleId = "1", preparingState = PlayerState.Ready };

        var monopoly = new MonopolyBuilder()
            .WithPlayer(A.Id, pa => pa.WithLocation(A.locationId).WithRole(A.roleId).WithState(PlayerState.Normal))
            .WithGameStage(GameStage.Preparing)
            .Build();

        // Act 
        monopoly.PlayerPrepare(A.Id);

        // Assert
        Assert.AreEqual(A.preparingState, monopoly.Players.First(p => p.Id == A.Id).State);

        monopoly.DomainEvents.NextShouldBe(new PlayerReadyEvent(A.Id, A.preparingState.ToString()));
    }

    [TestMethod]
    [Description(
        """
        Given:  房主為 A
                A: 位置1, 角色A, 已準備
                B: 位置未選擇
        When:   B 按下準備
        Then:   提醒無法準備, B 的準備狀態:準備中
        DomainEvent: B 的準備狀態:準備中
        """)]
    public void 玩家未選擇位置按下準備()
    {
        // Arrange
        var A = new { Id = "A", roleId = "A", locationId = 1, preparedState = PlayerState.Normal };
        var B = new { Id = "B", roleId = "B", locationId = 0, preparingState = PlayerState.Ready };

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
            .Build();

        // Act 
        monopoly.PlayerPrepare(B.Id);

        // Assert
        Assert.AreEqual(B.preparingState, monopoly.Players.First(p => p.Id == B.Id).State);

        monopoly.DomainEvents.NextShouldBe(new PlayerCannotReadyEvent(B.Id, B.preparingState.ToString(), B.roleId, B.locationId));
    }

    [TestMethod]
    [Description(
        """
        Given:  房主為 A
                A: 位置1, 角色A, 已準備
                B: 位置2, 角色未選擇
        When:   B 按下準備
        Then:   提醒無法準備, B 的準備狀態:準備中
        DomainEvent: B 的準備狀態:準備中
        """)]
    public void 玩家未選擇角色按下準備()
    {
        // Arrange
        var A = new { Id = "A", roleId = "A", locationId = 1, preparedState = PlayerState.Normal };
        var B = new { Id = "B", locationId = 0, preparingState = PlayerState.Ready };

        var monopoly = new MonopolyBuilder()
            .WithPlayer(A.Id, pa => pa.WithLocation(A.locationId)
                                        .WithState(A.preparedState)
                                        .WithRole(A.roleId)
                                        )
            .WithPlayer(B.Id, pb => pb.WithLocation(B.locationId)
                                        .WithState(B.preparingState)
                                        )
            .WithGameStage(GameStage.Preparing)
            .Build();

        // Act 
        monopoly.PlayerPrepare(B.Id);

        // Assert
        Assert.AreEqual(B.preparingState, monopoly.Players.First(p => p.Id == B.Id).State);

        monopoly.DomainEvents.NextShouldBe(new PlayerCannotReadyEvent(B.Id, B.preparingState.ToString(), null, B.locationId));
    }
}