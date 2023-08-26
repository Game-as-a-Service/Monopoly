using Domain.Common;
using Domain.Events;
using Domain.Interfaces;

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
    public bool EnableUpgrade { get; set; }
    public bool IsHost { get; set; }
    public int SuspendRounds { get; private set; } = 0;

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

    public void SuspendRound(string reason)
    {
        SuspendRounds = reason switch
        {
            "Jail" => 2,
            "ParkingLot" => 1,
            _ => 0,
        };
    }

    public List<DomainEvent> EndRound()
    {
        List<DomainEvent> events = new();

        mortgages.ForEach(m =>
        {
            events.AddRange(m.EndRound());
        });
        mortgages.RemoveAll(m => m.Deadline == 0);

        return events;
    }

    public List<DomainEvent> StartRound()
    {
        EndRoundFlag = true;
        EnableUpgrade = false;
        List<DomainEvent> events = new();

        if (SuspendRounds > 0)
        {
            SuspendRounds--;
            events.Add(new SuspendRoundEvent(Monopoly.Id, Id, SuspendRounds));
        }

        return events;
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
        if (mortgages.Exists(m => m.LandContract.Land.Id == landId))
        {
            return new PlayerCannotMortgageEvent(Monopoly.Id, Id, Money, landId);
        }
        else
        {
            var landContract = _landContractList.First(l => l.Land.Id == landId);
            mortgages.Add(new Mortgage(this, landContract));
            Money += landContract.Land.GetPrice("Mortgage");
            return new PlayerMortgageEvent(Monopoly.Id, Id, Money,
                                            mortgages[^1].LandContract.Land.Id,
                                            mortgages[^1].Deadline);
        }
    }

    internal DomainEvent RedeemLandContract(string landId)
    {
        if (mortgages.Exists(m => m.LandContract.Land.Id == landId))
        {
            var landContract = _landContractList.First(l => l.Land.Id == landId);
            if (Money >= landContract.Land.GetPrice("Redeem"))
            {
                Money -= landContract.Land.GetPrice("Redeem");
                mortgages.RemoveAll(m => m.LandContract.Land.Id == landId);
                return new PlayerRedeemEvent(Monopoly.Id, Id, Money, landId);
            }
            else
            {
                return new PlayerTooPoorToRedeemEvent(Monopoly.Id, Id, Money, landId, landContract.Land.GetPrice("Redeem"));
            }
        }
        else
        {
            return new LandNotInMortgageEvent(Monopoly.Id, Id, landId);
        }
    }

    #region 測試用
    public void MortgageForTest(string landId, int deadLine)
    {
        var landContract = _landContractList.First(l => l.Land.Id == landId);
        mortgages.Add(new Mortgage(this, landContract));
        mortgages[^1].SetDeadLine(deadLine);
    }
    #endregion

    public void PayToll(Player payee, decimal amount)
    {
        Money -= amount;
        payee.Money += amount;
    }

    internal DomainEvent BuildHouse()
    {
        Block block = Chess.CurrentBlock;

        if (block is Land land && EnableUpgrade)
        {
            return land.BuildHouse(this);
        }
        return new PlayerCannotBuildHouseEvent(Monopoly.Id, Id, block.Id);
    }

    internal List<DomainEvent> BuyLand(string BlockId)
    {
        List<DomainEvent> events = new();

        //判斷是否為空土地
        if (Chess.CurrentBlock is Land land)
        {
            if (land.GetOwner() is not null)
            {
                events.Add(new PlayerBuyBlockOccupiedByOtherPlayerEvent(Monopoly.Id, Id, BlockId));
            }
            else
            {
                //判斷格子購買金額足夠
                if (land.Price > Money)
                {
                    events.Add(new PlayerBuyBlockInsufficientFundsEvent(Monopoly.Id, Id, BlockId, land.Price));
                } 
                else
                {
                    //玩家扣款
                    Money -= land.Price;

                    //過戶(?
                    var landContract = new LandContract(this, land);
                    AddLandContract(landContract);

                    events.Add(new PlayerBuyBlockEvent(Monopoly.Id, Id, BlockId));
                }
            }
        }
        return events;
    }
}