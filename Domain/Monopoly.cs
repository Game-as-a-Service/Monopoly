using Domain.Common;
using Domain.Interfaces;
using Domain.Maps;
using static Domain.Map;

namespace Domain;

public class Monopoly: AbstractAggregateRoot
{
    public string Id { get; set; }
    public int[]? CurrentDice { get; set; } = null;
    public Player? CurrentPlayer { get; set; }
    public IDice[] Dices { get; init; }

    private readonly Map _map;
    private readonly List<Player> _players = new();
    private readonly Dictionary<Player, int> _playerRankDictionary = new(); // 玩家名次 {玩家,名次}

    public IDictionary<Player, int> PlayerRankDictionary => _playerRankDictionary.AsReadOnly();

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
    public void PlayerSelectDirection(Player player, Direction direction)
    {
        player.SelectDirection(direction);
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
    }

    public void EndAuction()
    {
        CurrentPlayer.Auction.End();
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
        CurrentPlayer.Auction.Bid(player, price);
    }

    public void MortgageLandContract(string playerId, string landId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        player.MortgageLandContract(landId);
    }

    public Player? GetOwner(Land location)
    {
        return location.GetOwner();
    }

    public bool CalculateToll(Land location, Player payer, Player payee, out decimal amount)
    {
        return location.CalculateToll(payer, payee, out amount);
    }

    public void PayToll(Player payer, Player payee, decimal amount)
    {
        payer.PayToll(payee, amount);
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
        var player = _players.Find(p => p.Id == id);
        if (player is null)
        {
            throw new Exception("找不到玩家");
        }

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
    /// <param name="player">購買玩家</param>
    /// <param name="BlockId">購買土地ID</param>
    public void BuyLand(Player player, string BlockId)
    {
        //判斷是否踩在該土地
        if (player.Chess.CurrentBlock.Id != BlockId) throw new Exception("必須在購買的土地上才可以購買");

        //判斷是否為空土地
        if (this.FindPlayerByLandId(BlockId) != null) throw new Exception("非空地");

        //判斷格子購買金額足夠
        var land = _map.FindBlockById(BlockId) as Land;
        if (land.Price > player.Money) throw new Exception("金額不足");

        //玩家扣款
        player.Money -= land.Price;

        //過戶(?
        var landContract = new LandContract(player, land);
        player.AddLandContract(landContract);
    }

    private Player? FindPlayerByLandId(string blockId)
    {
        return _players.Where(p => p.LandContractList.Any(l => l.Land.Id.Equals(blockId))).FirstOrDefault();
    }
}