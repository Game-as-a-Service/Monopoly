using static Shared.Domain.Map;

namespace Shared.Domain;

public class Game
{
    public string Id { get; init; }
    public int[]? CurrentDice { get; set; } = null;
    public int CurrentDiceTotal => CurrentDice?.Sum() ?? 0;
    public Player? CurrentPlayer { get; set; }
    public DiceSetting DiceSetting { get; init; } 

    private readonly Map _map;
    private readonly List<Player> _players = new();
    private readonly Dictionary<Player, int> _playerRankDictionary = new(); // 玩家名次 {玩家,名次}
    private readonly Random _random;

    public IDictionary<Player, (Block block, Direction direction)> PlayerPositionDictionary => _map.PlayerPositionDictionary;
    public IDictionary<Player, int> PlayerRankDictionary => _playerRankDictionary.AsReadOnly();
    public IReadOnlyList<Player> Players => _players.AsReadOnly();

    // 初始化遊戲
    public Game(string id, Map? map = null, DiceSetting? diceSetting = null)
    {
        Id = id;
        _map = map ?? new Map(Array.Empty<Block[]>());
        _random = new Random(id.GetHashCode());

        DiceSetting = diceSetting ?? new DiceSetting();
    }

    public void AddPlayer(Player player)
    {
        _players.Add(player);
    }

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

    public Block GetPlayerPosition(string playerId) 
    {
        var player = _players.Find(p => p.Id == playerId);
        if (player is null)
        {
            throw new Exception("找不到玩家");
        }
        return _map.GetPlayerPositionAndDirection(player).block;
    } 

    // 玩家選擇方向
    // 1.不能選擇回頭的方向
    // 2.不能選擇沒有的方向
    public void PlayerSelectDirection(Player player, Direction direction)
    {
        var (block, currentDirection) = _map.GetPlayerPositionAndDirection(player);

        if (direction == currentDirection.Opposite())
        {
            throw new Exception("不能選擇原本的方向");
        }
        new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right }.ToList().ForEach(d =>
        {
            if (d == direction && block.GetDirectionBlock(d) is null)
            {
                throw new Exception("不能選擇沒有的方向");
            }
        });
        _map.SetPlayerToBlock(player, block.Id, direction);
    }

    public Direction GetPlayerDirection(string playerId)
    {
        var player = _players.Find(p => p.Id == playerId);
        if (player is null)
        {
            throw new Exception("找不到玩家");
        }
        return _map.GetPlayerPositionAndDirection(player).direction;
    }

    public void Initial()
    {
        // 初始化玩家位置
        foreach (var player in _players)
        {
            _map.SetPlayerToBlock(player, "Start", Direction.Right);
        }
        // 初始化目前玩家
        CurrentPlayer = _players[0];
    }

    public void PlayerRollDice(string playerId)
    {
        var player = _players.Find(p => p.Id == playerId);
        if (player is null)
        {
            throw new Exception("找不到玩家");
        }
        if (player != CurrentPlayer)
        {
            throw new Exception("不是該玩家的回合");
        }

        CurrentDice = RollDice();
    }

    public void PlayerMoveChess(string playerId)
    {
        var player = _players.Find(p => p.Id == playerId);
        if (player is null)
        {
            throw new Exception("找不到玩家");
        }
        if (player != CurrentPlayer)
        {
            throw new Exception("不是該玩家的回合");
        }
        _map.PlayerMove(player, CurrentDiceTotal);
    }

    #region Private Functions
    /// <summary>
    /// 根據設定來擲骰子
    /// </summary>
    /// <returns>所有的骰子點數</returns>
    private int[] RollDice()
    {
        return Enumerable.Range(0, DiceSetting.NumberOfDice)
            .Select(_ => _random.Next(DiceSetting.Min, DiceSetting.Max + 1))
            .ToArray();
    }

    private void AddPlayerToRankList(Player player)
    {
        foreach (var rank in _playerRankDictionary)
        {
            _playerRankDictionary[rank.Key] += 1;
        }
        _playerRankDictionary.Add(player, 1);
    }
    #endregion
}
