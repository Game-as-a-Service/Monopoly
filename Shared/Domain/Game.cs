using static Shared.Domain.Map;

namespace Shared.Domain;

public class Game
{
    private readonly Map _map;

    private readonly List<Player> _players = new();
    private readonly Dictionary<Player, int> _playerRankDictionary = new(); // 玩家名次 {玩家,名次}

    public IDictionary<Player, int> PlayerRankDictionary => _playerRankDictionary.AsReadOnly();

    public Game(Map? map = null){
        _map = map ?? new Map(new IBlock[0][]);
    }

    public void AddPlayer(Player player) => _players.Add(player);

    public void UpdatePlayerState(Player player)
    {
        player.UpdateState();
        
        if (player.IsBankrupt())
            AddPlayerToRankList(player);
    }

    public void Settlement()
    {
        // 玩家資產計算方式: 土地價格+升級價格+剩餘金額
        
        // 排序未破產玩家的資產並加入名次清單
        var playerList = from p in _players
            where !p.IsBankrupt()
            orderby p.Money + p.LandContractList.Sum(l => l.Price + (l.House * l.Price)) ascending
            select p;
        foreach (var player in playerList)
        {
            AddPlayerToRankList(player);
        }
    }

    public void SetPlayerToBlock(Player player, string blockId, Direction direction) => _map.SetPlayerToBlock(player, blockId, direction);

    public void PlayerMove(Player player, int moveCount) => _map.PlayerMove(player, moveCount);

    private void AddPlayerToRankList(Player player)
    {
        foreach(var rank in _playerRankDictionary){
            _playerRankDictionary[rank.Key] += 1;
        }
        _playerRankDictionary.Add(player, 1);
    }

    public IBlock GetPlayerPosition(Player player) => _map.GetPlayerPositionAndDirection(player).block;

    // 玩家選擇方向
    // 1.不能選擇回頭的方向
    // 2.不能選擇沒有的方向
    public void SelectDirection(Player player, Direction direction)
    {
        var (block, currentDirection) = _map.GetPlayerPositionAndDirection(player);

        if (direction == currentDirection.Opposite())
        {
            throw new Exception("不能選擇原本的方向");
        }
        new [] {Direction.Up, Direction.Down, Direction.Left, Direction.Right}.ToList().ForEach(d =>
        {
            if (d == direction && block.GetDirectionBlock(d) is null)
            {
                throw new Exception("不能選擇沒有的方向");
            }
        });
        _map.SetPlayerToBlock(player, block.Id, direction);
    }

    public Direction GetPlayerDirection(Player player) => _map.GetPlayerPositionAndDirection(player).direction;

    public void Initial()
    {
        // 初始化玩家位置
        foreach (var player in _players)
        {
            _map.SetPlayerToBlock(player, "Start", Direction.Right);
        }
    }
}
