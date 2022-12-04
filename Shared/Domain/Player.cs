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

    public void AddMoney(int money)
    {
        this.money += money;
    }

    internal IList<LandContract> LandContractList => landContractList.AsReadOnly();
}
