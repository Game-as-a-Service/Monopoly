using Domain.Common;
using Domain.Events;

namespace Domain;

public class LandContract
{
    public int Deadline { get; private set; }

    public bool InMortgage { get; private set; } = false;

    public Player? Owner { get; set; }

    public Land Land { get; }

    public int House { get; private set; } = 0;

    public LandContract(Player? Owner, Land Land)
    {
        this.Owner = Owner;
        this.Land = Land;
    }

    internal DomainEvent EndRound()
    {
        if (InMortgage)
        {
            Deadline--;
            if (Deadline == 0)
            {
                Land.UpdateOwner(null);
                return new MortgageDueEvent(Owner.Id, Land.Id);
            }
            else
            {
                return new MortgageCountdownEvent(Owner.Id, Land.Id, Deadline);
            }
        }
        return DomainEvent.EmptyEvent;
    }

    internal void GetMortgage()
    {
        Deadline = 10;
        InMortgage = true;
    }

    internal void GetRedeem()
    {
        InMortgage = false;
    }

    internal void AddHouse(int house)
    {
        House += house;
    }

    #region 測試用
    public void SetDeadLine(int deadLine)
    {
        Deadline = deadLine;
    }
    #endregion
}