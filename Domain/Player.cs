using Domain.Common;
using Domain.Events;
using Domain.Interfaces;

namespace Domain;

public class Player
{
    private decimal money;
    private Chess chess;
    private readonly List<LandContract> _landContractList = new();

    public Player(string id, decimal money = 15000, bool isBankrupt = false, int bankruptRounds = 0)
    {
        Id = id;
        State = PlayerState.Normal;
        Money = money;
        State = isBankrupt ? PlayerState.Bankrupt : PlayerState.Normal;
        BankruptRounds = bankruptRounds;
    }

    public PlayerState State { get; private set; }
    public Monopoly Monopoly { get; internal set; }
    public string Id { get; }
    public decimal Money
    {
        get { return money; }
        set
        {
            money = (int)value;
            if (money < 0)
            {
                money = 0;
            }
        }
    }

    public IList<LandContract> LandContractList => _landContractList.AsReadOnly();

    public Chess Chess { get => chess; set => chess = value; }
    public bool EndRoundFlag { get; set; }
    // false: 回合尚不能結束，true: 玩家可結束回合
    public int SuspendRounds { get; private set; } = 0;
    public int BankruptRounds { get; set; }

    internal DomainEvent UpdateState()
    {
        if (Money <= 0 && !LandContractList.Any(l => !l.InMortgage))
        {
            State = PlayerState.Bankrupt;
            foreach (var landContract in LandContractList)
            {
                RemoveLandContract(landContract);
            }
            EndRoundFlag = true;
            return new BankruptEvent(Id);
        }
        return DomainEvent.EmptyEvent;
    }

    public bool IsBankrupt => State == PlayerState.Bankrupt;

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

        if (SuspendRounds > 0)
        {
            SuspendRounds--;
        }
    }

    internal void RollDice(Map map, IDice[] dices)
    {
        foreach (var dice in dices)
        {
            dice.Roll();
        }
        Monopoly.AddDomainEvent(new PlayerRolledDiceEvent(Id, dices.Sum(d => d.Value)));
        var events = chess.Move(map, dices.Sum(dice => dice.Value));

        Monopoly.AddDomainEvent(events);
    }

    internal void SelectDirection(Map map, Map.Direction direction)
    {
        var events = chess.ChangeDirection(map, direction);
        Monopoly.AddDomainEvent(events);
    }

    internal DomainEvent MortgageLandContract(string landId)
    {
        // 玩家擁有地契並尚未抵押
        if (_landContractList.Exists(l => l.Land.Id == landId && !l.InMortgage))
        {
            var landContract = _landContractList.First(l => l.Land.Id == landId);
            landContract.GetMortgage();
            Money += landContract.Land.GetPrice("Mortgage");
            return new PlayerMortgageEvent(Id, Money,
                                            landId,
                                            landContract.Deadline);
        }
        else
        {
            return new PlayerCannotMortgageEvent(Id, Money, landId);
        }
    }

    internal DomainEvent RedeemLandContract(string landId)
    {
        // 玩家擁有地契並正在抵押
        if (_landContractList.Exists(l => l.Land.Id == landId && l.InMortgage))
        {
            var landContract = _landContractList.First(l => l.Land.Id == landId);
            var RedeemMoney = landContract.Land.GetPrice("Redeem");
            if (Money >= RedeemMoney)
            {
                landContract.GetRedeem();
                Money -= RedeemMoney;
                return new PlayerRedeemEvent(Id, Money, landId);
            }
            else
            {
                return new PlayerTooPoorToRedeemEvent(Id, Money, landId, RedeemMoney);
            }
        }
        else
        {
            return new LandNotInMortgageEvent(Id, landId);
        }
    }

    internal void PayToll(Player owner, decimal amount)
    {
        Money -= amount;
        owner.Money += amount;
    }

    internal DomainEvent BuildHouse(Map map)
    {
        Block block = map.FindBlockById(chess.CurrentBlockId);
        if (block is not Land land)
        {
            return new PlayerCannotBuildHouseEvent(Id, block.Id);
        }
        // 玩家站在該土地上
        else if (Chess.CurrentBlockId != land.Id)
        {
            return new PlayerBuyBlockMissedLandEvent(Id, land.Id);
        }
        // 玩家擁有該土地
        else if (land.GetOwner() != this)
        {
            return new PlayerCannotBuildHouseEvent(Id, land.Id);
        }
        // 此土地沒有被抵押
        else if (LandContractList.Any(l => l.Land == land && l.InMortgage))
        {
            return new PlayerCannotBuildHouseEvent(Id, land.Id);
        }
        // 玩家有足夠的錢
        else if (land.UpgradePrice > Money)
        {
            return new PlayerTooPoorToBuildHouseEvent(Id, land.Id, Money, land.UpgradePrice);
        }
        // 玩家這回合沒有買地
        else if (Monopoly.CurrentPlayerState.IsBoughtLand)
        {
            return new PlayerCannotBuildHouseEvent(Id, land.Id);
        }
        // 玩家這回合沒有蓋房子
        else if (Monopoly.CurrentPlayerState.IsUpgradeLand)
        {
            return new PlayerCannotBuildHouseEvent(Id, land.Id);
        }
        else
        {
            return land.BuildHouse(this);
        }
    }

    internal List<DomainEvent> BuyLand(Map map, string BlockId)
    {
        List<DomainEvent> events = new();

        //判斷是否為可購買土地
        if (map.FindBlockById(chess.CurrentBlockId) is not Land land)
        {
            events.Add(new PlayerBuyBlockOccupiedByOtherPlayerEvent(Id, BlockId));
        }
        //判斷是否非空地
        else if (land.GetOwner() is not null)
        {
            events.Add(new PlayerBuyBlockOccupiedByOtherPlayerEvent(Id, BlockId));
        }
        //判斷是否踩在該土地
        else if (Chess.CurrentBlockId != BlockId)
        {
            events.Add(new PlayerBuyBlockMissedLandEvent(Id, BlockId));
        }
        //判斷格子購買金額足夠
        else if (land.Price > Money)
        {
            events.Add(new PlayerBuyBlockInsufficientFundsEvent(Id, BlockId, land.Price));
        }
        else
        {
            //玩家扣款
            Money -= land.Price;

            //過戶(?
            var landContract = new LandContract(this, land);
            AddLandContract(landContract);

            events.Add(new PlayerBuyBlockEvent(Id, BlockId));

        }

        return events;
    }

    internal bool CanNotSelectDirection(Map.Direction d)
    {
        return d == chess.CurrentDirection.Opposite();
    }
}