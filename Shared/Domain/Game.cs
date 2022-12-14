namespace Shared.Domain;

public class Game
{
    private readonly Map _map;
    public Game(Map? map = null){
        _map = map ?? new Map(new IBlock[0][]);
    }

    List<Player> players = new();

    public Dictionary<Player, int> RankList { get; set; } = new(); // 玩家名次 {玩家,名次}
    public Map Map => _map;

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public void SetState(Player player, PlayerState playerState)
    {
        player.SetState(playerState);
        //破產時，將名次加入名次清單
        if (playerState == PlayerState.Bankrupt)
        {
            AddPlayerToRankList(player);
        }
    }

    public void Settlement()
    {
        // 玩家資產計算方式: 土地價格+升級價格+剩餘金額
        
        // 排序未破產玩家的資產並加入名次清單
        var playerList = from p in players
            where !p.IsBankrupt()
            orderby p.Money + p.LandContractList.Sum(l => l.Price + (l.House * l.Price)) ascending
            select p;
        foreach (var player in playerList)
        {
            AddPlayerToRankList(player);
        }
    }

    private void AddPlayerToRankList(Player player)
    {
        foreach(var rank in RankList){
            RankList[rank.Key] += 1;
        }
        RankList.Add(player, 1);
    }

    // if not find return
    public Player? FindPlayerById(string id) => players.FirstOrDefault(p => p.Id == id);


}
