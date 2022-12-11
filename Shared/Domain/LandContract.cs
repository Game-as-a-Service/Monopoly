namespace Shared.Domain;

public class LandContract
{
    private int price;
    private int defaultPrice;
    private int sellPrice = 0;
    private Player? sellPlayer = null;
    private Player? owner;
    private int houseCount;
    private string id;

    public LandContract(int price, Player? owner, string id)
    {
        this.price = price;
        this.defaultPrice = (int)(price * 0.5);
        this.owner = owner;
        this.houseCount = 0;
        this.id = id;
    }

    public void SetOutcry(Player player, int price) {
        if (price > this.sellPrice && player.Money >= price) {
            this.sellPlayer = player;
            this.sellPrice = price;
        }
    }

    public bool HasOutcry() {
        return sellPrice != 0;
    }

    public void Sell() {
        int _price;
        Player? player = this.owner;

        if (this.sellPlayer != null) {
            _price = sellPrice;
            this.sellPrice = 0;
            this.owner = this.sellPlayer;
            this.owner?.AddLandContract(this);
            this.owner?.AddMoney(-_price);
            this.sellPlayer = null;
        } else {
            _price = (int)(price * 0.7);
            this.sellPrice = 0;
            this.owner = null;
            this.houseCount = 0;
        }

        player?.AddMoney(_price);
        player?.RemoveLandContract(this);
    }

    public int Price => price;

    public int House => houseCount;

    public string Id => id;

    public void Upgrade()
    {
        this.houseCount++;
    }
}