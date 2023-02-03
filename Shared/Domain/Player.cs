using Shared.Domain.Interfaces;

namespace Shared.Domain;

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
    public string Id { get; }
    public decimal Money { get; set; }

    public IList<LandContract> LandContractList => _landContractList.AsReadOnly();

    public Chess Chess { get => chess; set => chess = value; }
    public Auction Auction => auction;

    public void UpdateState()
    {
        if (Money <= 0 && LandContractList.Count == 0)
        {
            State = PlayerState.Bankrupt;
        }
    }

    public bool IsBankrupt() {
        return State == PlayerState.Bankrupt;
    }

    public void AddLandContract(LandContract landContract)
    {
        _landContractList.Add(landContract);
    }

    public void RemoveLandContract(LandContract landContract) {
        _landContractList.Remove(landContract);
    }

    public LandContract? FindLandContract(string id) {
        return LandContractList.Where(landContract => landContract.Land.Id == id).FirstOrDefault();
    }

    public void AddMoney(decimal money)
    {
        Money += money;
    }

    public void AuctionLandContract(string id) {
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
        chess.Move(dices.Sum(dice => dice.Value));

        return dices;
    }

    internal void SelectDirection(Map.Direction direction)
    {
        chess.ChangeDirection(direction);
    }

    internal void Outcry(int money)
    {
        throw new NotImplementedException();
    }
}
