using Domain.Builders;

namespace DomainTests.Testcases;

[TestClass]
public class SelectLocationTest
{
    [TestMethod]
    [Description("""
        Given:  玩家A:位置未選擇
        When:   玩家A選擇位置紅(位置ID=1)
        Then:   玩家A的位置為1
        """)]
    public void 玩家選擇沒人的位置()
    {
        // Arrange
        var A = new { Id = "A", locationId = 0, selectLocationId = 1 };
        var monopoly = new MonopolyBuilder()
            .WithPlayer(A.Id, pa => pa.WithLocation(A.locationId))
            .WithGameStage(GameStage.Preparing)
            .Build();

        // Act
        monopoly.SelectLocation(A.Id, A.selectLocationId);

        // Assert
        Assert.AreEqual(A.selectLocationId, monopoly.Players.First(p => p.Id == A.Id).LocationId);

    }

    [TestMethod]
    [Description("""
        Given:  玩家A:位置未選擇
                玩家B:位置1
        When:   玩家A選擇位置1
        Then:   玩家A的位置為未選擇
        """)]
    public void 玩家選擇有人的位置()
    {
        // Arrange
        var A = new { Id = "A", locationId = 0, selectLocationId = 1 };
        var B = new { Id = "B", locationId = 1 };
        var monopoly = new MonopolyBuilder()
            .WithPlayer(A.Id, pa => pa.WithLocation(A.locationId))
            .WithPlayer(B.Id, pa => pa.WithLocation(B.locationId))
            .WithGameStage(GameStage.Preparing)
            .Build();

        // Act
        monopoly.SelectLocation(A.Id, A.selectLocationId);

        // Assert
        Assert.AreEqual(A.locationId, monopoly.Players.First(p => p.Id == A.Id).LocationId);
    }

    [TestMethod]
    [Description("""
        Given:  玩家A:位置1
        When:   玩家A選擇位置2
        Then:   玩家A的位置為2
        """)]
    public void 有選位置的玩家更換位置()
    {
        // Arrange
        var A = new { Id = "A", locationId = 1, selectLocationId = 2 };
        var monopoly = new MonopolyBuilder()
            .WithPlayer(A.Id, pa => pa.WithLocation(A.locationId))
            .WithGameStage(GameStage.Preparing)
            .Build();

        // Act
        monopoly.SelectLocation(A.Id, A.selectLocationId);

        // Assert
        Assert.AreEqual(A.selectLocationId, monopoly.Players.First(p => p.Id == A.Id).LocationId);
    }
}