using Shared.Domain.Interfaces;

namespace Shared.Domain;

public class Player
{
    private Chess chess;
    private List<LandContract> landContractList = new();

    public Player(string id, int money = 15000)
    {
        Id = id;
        State = PlayerState.Normal;
        Money = money;
    }

    public PlayerState State { get; private set; }
    public string Id { get; }
    public int Money { get; set; }

    public IList<LandContract> LandContractList => landContractList.AsReadOnly();

    public Chess Chess { get => chess; set => chess = value; }

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
        this.landContractList.Add(landContract);
    }

    public void RemoveLandContract(LandContract landContract) {
        this.landContractList.Remove(landContract);
    }

    public bool FindLandContract(string id) {
        return LandContractList.Where(landContract => landContract.Id == id).Count() == 1;
    }

    public void AddMoney(int money)
    {
        Money += money;
    }

    public LandContract SellLandContract(string id) {
        var landContract = landContractList.Where(land => land.Id == id);

        return landContract.First();
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
}
