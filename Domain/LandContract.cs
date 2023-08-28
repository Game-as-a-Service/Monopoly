using Domain.Common;
using Domain.Events;

namespace Domain;

public class LandContract
{
    public int Deadline { get; private set; }

    internal bool Mortgage { get; private set; } = false;

    public Player? Owner { get; set; }

    public Land Land { get; }

    public LandContract(Player? Owner, Land Land)
    {
        this.Owner = Owner;
        this.Land = Land;
    }

    internal DomainEvent EndRound()
    {
        if(Mortgage)
        {
            Deadline--;
            if (Deadline == 0)
            {
                Land.UpdateOwner(null);
                return new MortgageDueEvent(Owner.Monopoly.Id, Owner.Id, Land.Id);
            }
            else
            {
                return new MortgageCountdownEvent(Owner.Monopoly.Id, Owner.Id, Land.Id, Deadline);
            }
        }
        return DomainEvent.EmptyEvent;
    }

    internal void GetMortgage()
    {
        Deadline = 10;
        Mortgage = true;
    }

    internal void GetRedeem()
    {
        Mortgage = false;
    }

    #region 測試用
    public void SetDeadLine(int deadLine)
    {
        Deadline = deadLine;
    }
    #endregion
}