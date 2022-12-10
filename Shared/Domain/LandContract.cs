namespace Shared.Domain;

public class LandContract
{
    private int price;
    private int defaultPrice = 0;
    private int sellPrice = 0;
    private Player? owner;
    private int houseCount;
    private string id;

    public LandContract(int price, Player? owner, string id)
    {
        this.price = price;
        this.owner = owner;
        this.houseCount = 0;
        this.id = id;
    }

    public LandContract DefaultPrice(int price) {
        this.defaultPrice = price;
        return this;
    }

    public bool HasOutcry() {
        if (sellPrice == 0) return false;
        return true;
    }

    public int Sell(string id, Player? player) {
        int? _price;
        switch (id) {
            case "system":
                _price = (int)(price * 0.7);
                defaultPrice = 0;
                sellPrice = 0;
                owner = null;
                houseCount = 0;
                break;
            default:
                _price = sellPrice;
                defaultPrice = 0;
                sellPrice = 0;
                owner = player;
                break;
        }

        return (int)_price;
    }

    public int Price => price;

    public int House => houseCount;

    public string Id => id;

    public void Upgrade()
    {
        this.houseCount++;
    }
}