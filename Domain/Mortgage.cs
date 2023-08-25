using Domain.Common;
using Domain.Events;

namespace Domain;

public class Mortgage
{
    private Player player;
    private LandContract landContract;
    private int deadline;

    public Mortgage(Player player, LandContract landContract)
    {
        this.player = player;
        this.landContract = landContract;
        deadline = 10;
    }

    public LandContract LandContract => landContract;
    public int Deadline => deadline;

    public IEnumerable<DomainEvent> EndRound()
    {
        deadline --;
        if (deadline == 0)
        {
            landContract.Owner.RemoveLandContract(landContract);
            yield return new MorgageDueEvent(player.Monopoly.Id, player.Id, landContract.Land.Id);
        }
        else
        {
            yield return new MorgageCountdownEvent(player.Monopoly.Id, player.Id, landContract.Land.Id, deadline);
        }
    }

    #region 測試用
    public void SetDeadLine(int deadLine)
    {
        this.deadline = deadLine;
    }
    #endregion
}