using Domain.Events;
using System.Numerics;
using Domain.Interfaces;
using Domain.Common;

namespace Domain;

public class Player
{
    private Chess chess;
    private readonly List<LandContract> _landContractList = new();
    private Auction auction;
    private readonly List<Mortgage> mortgages;

    public Player(string id, decimal money = 15000)
    {
        Id = id;
        State = PlayerState.Normal;
        Money = money;
        mortgages = new List<Mortgage>();
    }

    public PlayerState State { get; private set; }
    public Monopoly Monopoly { get; internal set; }
    public string Id { get; }
    public decimal Money { get; set; }

    public IList<LandContract> LandContractList => _landContractList.AsReadOnly();

    public Chess Chess { get => chess; set => chess = value; }
    public Auction Auction => auction;
    public IList<Mortgage> Mortgage => mortgages.AsReadOnly();
    public bool EndRoundFlag { get; set; }
    public bool IsHost { get; set; }

    // false: 回合尚不能結束，true: 玩家可結束回合

    public void UpdateState()
    {
        if (Money <= 0 && LandContractList.Count == 0)
        {
            State = PlayerState.Bankrupt;
        }
    }

    public bool IsBankrupt()
    {
        return State == PlayerState.Bankrupt;
    }

    public void AddLandContract(LandContract landContract)
    {
        _landContractList.Add(landContract);
        landContract.Land.UpdateOwner(this);
    }

    public void RemoveLandContract(LandContract landContract)
    {
        _landContractList.Remove(landContract);
    }

    public LandContract? FindLandContract(string id)
    {
        return LandContractList.Where(landContract => landContract.Land.Id == id).FirstOrDefault();
    }

    public void AuctionLandContract(string id)
    {
        var landContract = _landContractList.Where(landContract => landContract.Land.Id == id).FirstOrDefault();
        if (landContract is null)
            throw new Exception("找不到地契");
        auction = new Auction(landContract);
    }

    internal IDice[] RollDice(IDice[] dices)
    {
        EndRoundFlag = true;

        foreach (var dice in dices)
        {
            dice.Roll();
        }

        var events = chess.Move(dices.Sum(dice => dice.Value));
        events.AddRange(chess.GetLandEvent());

        Monopoly.AddDomainEvent(events);
        return dices;
    }

    internal void SelectDirection(Map.Direction direction)
    {
        var events = chess.ChangeDirection(direction);
        Monopoly.AddDomainEvent(events);
    }

    internal DomainEvent MortgageLandContract(string landId)
    {
        if(mortgages.Exists(m => m.LandContract.Land.Id == landId))
        {
            return new PlayerCannotMortgageEvent(Monopoly.Id, Id, Money, landId);
        }
        else{
            var landContract = _landContractList.First(l => l.Land.Id == landId);
            mortgages.Add(new Mortgage(this, landContract));
            Money += landContract.Land.GetMortgagePrice();
            return new PlayerMortgageEvent(Monopoly.Id, Id, Money, 
                                            mortgages[mortgages.Count-1].LandContract.Land.Id, 
                                            mortgages[mortgages.Count-1].Deadline);
        }
    }

    #region 測試用
    public void MortgageForTest(string landId)
    {
        var landContract = _landContractList.First(l => l.Land.Id == landId);
        mortgages.Add(new Mortgage(this, landContract));
    }

    #endregion

    public void PayToll(Player payee, decimal amount)
    {
        Money -= amount;
        payee.Money += amount;
    }

    public DomainEvent BuildHouse()
    {
        Block block = Chess.CurrentBlock;

        if(block is Land land)
        {
            return land.BuildHouse(this);
        }
        return new PlayerCannotBuildHouseEvent(Monopoly.Id, Id, block.Id);
    }
}