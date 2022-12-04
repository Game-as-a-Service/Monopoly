namespace Shared.Domain;

public class LandContract
{
    private int price;
    private Player? owner;
    private int houseCount;

    public LandContract(int price, Player? owner)
    {
        this.price = price;
        this.owner = owner;
        this.houseCount = 0;
    }

    public int Price => price;

    public int House => houseCount;

    public void Upgrade()
    {
        this.houseCount++;
    }
}