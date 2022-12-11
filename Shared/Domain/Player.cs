namespace Shared.Domain;

public class Player
{
    private List<LandContract> landContractList = new();
    private int money;

    public Player(string id)
    {
        Id = id;
    }

    public PlayerState State { get; private set; } = PlayerState.Normal;
    public string Id { get; }
    public int Money => money;

    public void SetState(PlayerState playerState)
    {
        State = playerState;
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

    public bool FindLAndContract(string id) {
        return LandContractList.Where(landContract => landContract.Id == id).Count() == 1;
    }

    public void AddMoney(int money)
    {
        this.money += money;
    }

    public LandContract SellLandContract(string id) {
        var landContract = landContractList.Where(land => land.Id == id);

        return landContract.First();
    }

    internal IList<LandContract> LandContractList => landContractList.AsReadOnly();
}
