using Domain.Builders;
using Domain.Events;

namespace DomainTests.Testcases;

[TestClass]
public class SettlementTest
{
    [TestMethod]
    [Description(
        """
        Given:  遊戲目前第 7 回合
                A 持有 5000 元
                B 第 6 回合破產
                C 第 7 回合破產
        When:   系統進行遊戲結算
        Then:   A 第一名
                B 第三名
                C 第二名

                DomainEvent:    遊戲結束，7 回合，名次為 A C B，A 剩餘 5000 元，C 第 7 回合破產，B 第 6 回合破產
        """)]
    public void 因為有人破產而進行遊戲結算()
    {
        // Arrange
        var A = new { Id = "A", Money = 5000 };
        var B = new { Id = "B", Money = 0, BankruptRound = 6 };
        var C = new { Id = "C", Money = 0, BankruptRound = 7 };
        var expect = new { Rounds = 7 };

        var monopoly = new MonopolyBuilder()
            .WithRounds(7)
            .WithPlayer(A.Id, a => a.WithMoney(A.Money))
            .WithPlayer(B.Id, b => b.WithMoney(B.Money).WithBankrupt(B.BankruptRound))
            .WithPlayer(C.Id, c => c.WithMoney(C.Money).WithBankrupt(C.BankruptRound))
            .Build();

        // Act
        monopoly.Settlement();

        // Assert
        var playerA = monopoly.Players.First(p => p.Id == A.Id);
        var playerB = monopoly.Players.First(p => p.Id == B.Id);
        var playerC = monopoly.Players.First(p => p.Id == C.Id);
        monopoly.DomainEvents.NextShouldBe(new GameSettlementEvent(expect.Rounds, playerA, playerC, playerB));
    }

    [TestMethod]
    public void 玩家ABCD_遊戲時間結束_A的結算金額為5000_B的結算金額為4000_C的結算金額為3000_D的結算金額為2000__當遊戲結算__名次為ABCD()
    {
    }
}