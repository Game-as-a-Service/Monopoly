namespace Domain;

public class Mortgage
{
    private Player player;
    private LandContract landContract;
    private int deadline;

    public Mortgage(Player player, LandContract landContract)
    {
        this.player = player;
        this.landContract = landContract;
        deadline = 10;
    }

    public LandContract LandContract => landContract;
    public int Deadline => deadline;

    #region 測試用
    public void SetDeadLine(int deadLine)
    {
        this.deadline = deadLine;
    }
    #endregion
}