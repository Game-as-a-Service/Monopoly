using Domain.Common;
using Domain.Events;
using Domain.Interfaces;
using Domain.Maps;
using static Domain.Map;

namespace Domain;

public class Monopoly : AbstractAggregateRoot
{
    public string Id { get; set; }
    public int[]? CurrentDice { get; set; } = null;
    public Player? CurrentPlayer { get; set; }
    public IDice[] Dices { get; init; }

    private readonly Map _map;
    private readonly List<Player> _players = new();
    private readonly Dictionary<Player, int> _playerRankDictionary = new(); // 玩家名次 {玩家,名次}

    public IDictionary<Player, int> PlayerRankDictionary => _playerRankDictionary.AsReadOnly();

    public ICollection<Player> Players => _players.AsReadOnly();

    public string Host => _players.Find(p => p.IsHost)!.Id;

    // 初始化遊戲
    public Monopoly(string id, Map? map = null, IDice[]? dices = null)
    {
        Id = id;
        _map = map ?? new SevenXSevenMap();
        Dices = dices ?? new IDice[2] { new Dice(), new Dice() };
    }

    public void AddPlayer(Player player, string blockId = "Start", Direction direction = Direction.Right)
    {
        Block block = _map.FindBlockById(blockId);
        Chess chess = new(player, _map, block, direction);
        player.Chess = chess;
        player.Chess.SetBlock(blockId, direction);
        player.Monopoly = this;
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
                         orderby p.Money + p.LandContractList.Sum(l => (l.Land.House + 1) * l.Land.Price) ascending
                         select p;
        foreach (var player in playerList)
        {
            AddPlayerToRankList(player);
        }
    }

    public Block GetPlayerPosition(string playerId)
    {
        Player player = GetPlayer(playerId);
        return player.Chess.CurrentBlock;
    }

    // 玩家選擇方向
    // 1.不能選擇回頭的方向
    // 2.不能選擇沒有的方向
    public void PlayerSelectDirection(string playerId, string direction)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        var d = GetDirection(direction);
        player.SelectDirection(d);
    }

    private static Direction GetDirection(string direction)
    {
        return direction switch
        {
            "Up" => Direction.Up,
            "Down" => Direction.Down,
            "Left" => Direction.Left,
            "Right" => Direction.Right,
            _ => throw new Exception("方向錯誤")
        };
    }

    public Direction GetPlayerDirection(string playerId)
    {
        Player player = GetPlayer(playerId);
        return player.Chess.CurrentDirection;
    }

    public void Initial()
    {
        // 初始化目前玩家
        CurrentPlayer = _players[0];
    }

    /// <summary>
    /// 擲骰子
    /// 並且移動棋子直到遇到需要選擇方向的地方
    /// </summary>
    /// <param name="playerId"></param>
    /// <exception cref="Exception"></exception>
    public void PlayerRollDice(string playerId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        IDice[] dices = player.RollDice(Dices);
        AddDomainEvent(new PlayerRolledDiceEvent(Id, playerId, dices.Sum(d => d.Value)));
    }

    public void EndAuction()
    {
        CurrentPlayer?.Auction.End();
    }

    public void PlayerSellLandContract(string playerId, string landId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        player.AuctionLandContract(landId);
    }

    public void PlayerBid(string playerId, int price)
    {
        Player player = GetPlayer(playerId);
        CurrentPlayer?.Auction.Bid(player, price);
    }

    public void MortgageLandContract(string playerId, string landId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        AddDomainEvent(player.MortgageLandContract(landId));
    }

    public void PayToll(string payerId)
    {
        Player payer = GetPlayer(payerId);
        Land location = (Land)GetPlayerPosition(payer.Id);

        var domainEvent = location.PayToll(payer);

        AddDomainEvent(domainEvent);
    }

    public void BuildHouse(string playerId)
    {
        Player player = GetPlayer(playerId);

        AddDomainEvent(player.BuildHouse());
    }

    #region Private Functions

    private void AddPlayerToRankList(Player player)
    {
        foreach (var rank in _playerRankDictionary)
        {
            _playerRankDictionary[rank.Key] += 1;
        }
        _playerRankDictionary.Add(player, 1);
    }

    private Player GetPlayer(string id)
    {
        var player = _players.Find(p => p.Id == id) ?? throw new Exception("找不到玩家");
        return player;
    }

    private void VerifyCurrentPlayer(Player? player)
    {
        if (player != CurrentPlayer)
        {
            throw new Exception("不是該玩家的回合");
        }
    }

    #endregion Private Functions

    /// <summary>
    /// 購買土地
    /// </summary>
    /// <param name="playerId">購買玩家ID</param>
    /// <param name="BlockId">購買土地ID</param>
    public void BuyLand(string playerId, string BlockId)
    {
        BuyLand(GetPlayer(playerId), BlockId);
    }

    /// <summary>
    /// 購買土地
    /// </summary>
    /// <param name="player">購買玩家</param>
    /// <param name="BlockId">購買土地ID</param>
    public void BuyLand(Player player, string BlockId)
    {
        var domainEvent = new List<DomainEvent>();

        #region 檢核

        //判斷是否踩在該土地
        if (player.Chess.CurrentBlock.Id != BlockId)
        {
            domainEvent.Add(new PlayerBuyBlockMissedLandEvent(Id, player.Id, BlockId));
        }

        //判斷是否為空土地
        if (FindPlayerByLandId(BlockId) != null)
        {
            domainEvent.Add(new PlayerBuyBlockOccupiedByOtherPlayerEvent(Id, player.Id, BlockId));
        }

        //判斷格子購買金額足夠
        var land = _map.FindBlockById(BlockId) as Land;
        if (land.Price > player.Money)
        {
            domainEvent.Add(new PlayerBuyBlockInsufficientFundsEvent(Id, player.Id, BlockId, land.Price));
        }

        #endregion 檢核

        if (domainEvent.Count <= 0)
        {
            //玩家扣款
            player.Money -= land.Price;

            //過戶(?
            var landContract = new LandContract(player, land);
            player.AddLandContract(landContract);

            domainEvent.Add(new PlayerBuyBlockEvent(Id, player.Id, BlockId));
        }

        AddDomainEvent(domainEvent);
    }

    private Player? FindPlayerByLandId(string blockId)
    {
        return _players.FirstOrDefault(p => p.LandContractList.Any(l => l.Land.Id.Equals(blockId)));
    }
}