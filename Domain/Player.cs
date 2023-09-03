using Domain.Common;
using Domain.Events;
using Domain.Interfaces;

namespace Domain;

public class Player
{
    private Chess chess;
    private readonly List<LandContract> _landContractList = new();
    private Auction auction;

    public Player(string id, decimal money = 15000)
    {
        Id = id;
        State = PlayerState.Normal;
        Money = money;
    }

    public PlayerState State { get; private set; }
    public Monopoly Monopoly { get; internal set; }
    public string Id { get; }
    public decimal Money { get; set; }

    public IList<LandContract> LandContractList => _landContractList.AsReadOnly();

    public Chess Chess { get => chess; set => chess = value; }
    public Auction Auction => auction;
    public bool EndRoundFlag { get; set; }
    // false: 回合尚不能結束，true: 玩家可結束回合
    public bool EnableUpgrade { get; set; }
    public int SuspendRounds { get; private set; } = 0;

    

    internal DomainEvent UpdateState()
    {
        if (Money <= 0 && !LandContractList.Any(l => !l.Mortgage))
        {
            State = PlayerState.Bankrupt;
            foreach(var landContract in LandContractList)
            {
                RemoveLandContract(landContract);
            }
            EndRoundFlag = true;
            return new BankruptEvent(Monopoly.Id, Id);
        }
        return DomainEvent.EmptyEvent;
    }

    internal bool IsBankrupt()
    {
        return State == PlayerState.Bankrupt;
    }

    public void AddLandContract(LandContract landContract)
    {
        _landContractList.Add(landContract);
        landContract.Land.UpdateOwner(this);
    }

    internal void RemoveLandContract(LandContract landContract)
    {
        landContract.Land.UpdateOwner(null);
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

        _landContractList.ForEach(l =>
        {
            events.Add(l.EndRound());
        });
        _landContractList.RemoveAll(l => l.Deadline == 0);

        return events;
    }

    internal void StartRound()
    {
        EndRoundFlag = true;
        EnableUpgrade = false;

        if (SuspendRounds > 0)
        {
            SuspendRounds--;
        }
    }

    public void AuctionLandContract(string id)
    {
        var landContract = _landContractList.Where(landContract => landContract.Land.Id == id).FirstOrDefault();
        if (landContract is null)
            throw new Exception("找不到地契");
        auction = new Auction(landContract);
    }

    internal IDice[] RollDice(Map map, IDice[] dices)
    {
        foreach (var dice in dices)
        {
            dice.Roll();
        }

        var events = chess.Move(map, dices.Sum(dice => dice.Value));

        Monopoly.AddDomainEvent(events);
        return dices;
    }

    internal void SelectDirection(Map map, Map.Direction direction)
    {
        var events = chess.ChangeDirection(map, direction);
        Monopoly.AddDomainEvent(events);
    }

    internal DomainEvent MortgageLandContract(string landId)
    {
        // 玩家擁有地契並尚未抵押
        if(_landContractList.Exists(l => l.Land.Id == landId && !l.Mortgage))
        {
            var landContract = _landContractList.First(l => l.Land.Id == landId);
            landContract.GetMortgage();
            Money += landContract.Land.GetPrice("Mortgage");
            return new PlayerMortgageEvent(Monopoly.Id, Id, Money,
                                            landId,
                                            landContract.Deadline);
        }
        else
        {
            return new PlayerCannotMortgageEvent(Monopoly.Id, Id, Money, landId);
        }
    }

    internal DomainEvent RedeemLandContract(string landId)
    {
        // 玩家擁有地契並正在抵押
        if (_landContractList.Exists(l => l.Land.Id == landId && l.Mortgage))
        {
            var landContract = _landContractList.First(l => l.Land.Id == landId);
            if (Money >= landContract.Land.GetPrice("Redeem"))
            {
                landContract.GetRedeem();
                Money -= landContract.Land.GetPrice("Redeem");
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
        landContract.GetMortgage();
        landContract.SetDeadLine(deadLine);
    }
    #endregion

    internal void PayToll(Player owner, decimal amount)
    {
        Money -= amount;
        owner.Money += amount;
    }

    internal DomainEvent BuildHouse(Map map)
    {
        Block block = map.FindBlockById(chess.CurrentBlockId);

        if (block is Land land && EnableUpgrade)
        {
            return land.BuildHouse(this);
        }
        return new PlayerCannotBuildHouseEvent(Monopoly.Id, Id, block.Id);
    }

    internal List<DomainEvent> BuyLand(Map map, string BlockId)
    {
        List<DomainEvent> events = new();

        //判斷是否為空土地
        if (map.FindBlockById(chess.CurrentBlockId) is Land land)
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