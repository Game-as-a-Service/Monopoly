using Domain.Builders;
using Domain.Common;
using Domain.Events;
using Domain.Interfaces;
using static Domain.Map;

namespace Domain;

public class Monopoly : AbstractAggregateRoot
{
    public string Id { get; set; }
    public GameStage GameStage { get; }
    public int[]? CurrentDice { get; set; } = null;
    public CurrentPlayerState CurrentPlayerState => _currentPlayerState;
    public IDice[] Dices { get; init; }
    private Player CurrentPlayer => _players.First(p => p.Id == _currentPlayerState.PlayerId);

    private readonly Map _map;
    private readonly List<Player> _players = new();
    private CurrentPlayerState _currentPlayerState;

    public ICollection<Player> Players => _players.AsReadOnly();

    public string HostId { get; init; }

    public Map Map => _map;

    public int Rounds { get; private set; }


    internal Monopoly(string gameId, Player[] players, GameStage gameStage, Map map, string hostId, CurrentPlayerState currentPlayerState, IDice[]? dices = null, int rounds = 0)
    {
        Id = gameId;
        GameStage = gameStage;
        _players = players.ToList();
        _map = map;
        HostId = hostId;
        _currentPlayerState = currentPlayerState;
        Dices = dices ?? new IDice[2] { new Dice(), new Dice() };

        foreach (var player in _players)
        {
            player.Monopoly = this;
        }

        Rounds = rounds;
    }

    public void AddPlayer(Player player, string blockId = "Start", Direction direction = Direction.Right)
    {
        Chess chess = new(player, blockId, direction, 0);
        player.Chess = chess;
        player.Monopoly = this;
        _players.Add(player);
    }

    public void UpdatePlayerState(Player player)
    {
        AddDomainEvent(player.UpdateState());
    }

    public void Settlement()
    {
        // 玩家資產計算方式: 土地價格+升級價格+剩餘金額 
        // 抵押的房地產不列入計算
        var PropertyCalculate = (Player player) =>
            player.Money + player.LandContractList.Where(l => !l.InMortgage).Sum(l => (l.Land.House + 1) * l.Land.Price);
        // 根據玩家資產進行排序，多的在前，若都已經破產了，則以破產時間晚的在前
        var players = _players.OrderByDescending(PropertyCalculate).ThenByDescending(p => p.BankruptRounds).ToArray();
        AddDomainEvent(new GameSettlementEvent(Rounds, players));
    }

    public Block GetPlayerPosition(string playerId)
    {
        Player player = GetPlayer(playerId);
        return _map.FindBlockById(player.Chess.CurrentBlockId);
    }

    // 玩家選擇方向
    // 1.不能選擇回頭的方向
    // 2.不能選擇沒有的方向
    public void PlayerSelectDirection(string playerId, string direction)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        if (CurrentPlayerState.HadSelectedDirection)
        {
            AddDomainEvent(new PlayerHadSelectedDirectionEvent(playerId));
            return;
        }
        var d = GetDirection(direction);
        if (CurrentPlayer.CanNotSelectDirection(d))
        {
            AddDomainEvent(new PlayerChooseInvalidDirectionEvent(playerId, direction));
            return;
        }
        player.SelectDirection(_map, d);
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

    public void Initial()
    {
        // 初始化目前玩家
        _currentPlayerState = new CurrentPlayerStateBuilder(_players[0].Id).Build(null);
        CurrentPlayer.StartRound();
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
        player.RollDice(_map, Dices);
    }

    public void EndAuction()
    {
        AddDomainEvent(_currentPlayerState.Auction.End());
    }

    public void PlayerSellLandContract(string playerId, string landId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        var landContract = player.FindLandContract(landId) ?? throw new Exception("找不到地契");
        _currentPlayerState = CurrentPlayerState with { Auction = new Auction(landContract) };
    }

    public void PlayerBid(string playerId, decimal price)
    {
        Player player = GetPlayer(playerId);
        if (playerId == CurrentPlayer.Id)
        {
            AddDomainEvent(new CurrentPlayerCannotBidEvent(playerId));
        }
        else
        {
            AddDomainEvent(_currentPlayerState?.Auction.Bid(player, price));
        }
    }

    public void MortgageLandContract(string playerId, string landId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        AddDomainEvent(player.MortgageLandContract(landId));
    }

    public void RedeemLandContract(string playerId, string landId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        AddDomainEvent(player.RedeemLandContract(landId));
    }

    public void PayToll(string playerId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        Land location = (Land)GetPlayerPosition(player.Id);
        if (CurrentPlayerState.IsPayToll)
        {
            AddDomainEvent(new PlayerDoesntNeedToPayTollEvent(player.Id, player.Money));
            return;
        }

        var domainEvent = location.PayToll(player);

        AddDomainEvent(domainEvent);
    }

    public void BuildHouse(string playerId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        AddDomainEvent(player.BuildHouse(_map));
    }

    public void EndRound()
    {
        if (CurrentPlayerState.CanEndRound)
        {
            // 結束回合，輪到下一個玩家
            AddDomainEvent(CurrentPlayer.EndRound());
            string lastPlayerId = CurrentPlayer.Id;
            do
            {
                _currentPlayerState = new CurrentPlayerStateBuilder(_players[(_players.IndexOf(CurrentPlayer) + 1) % _players.Count].Id).Build(null);
                CurrentPlayer.StartRound();
            } while (CurrentPlayer.State == PlayerState.Bankrupt);
            AddDomainEvent(new EndRoundEvent(lastPlayerId, CurrentPlayer.Id));
            if (CurrentPlayer.SuspendRounds > 0)
            {
                _currentPlayerState = _currentPlayerState with { IsPayToll = true }; // 當玩家暫停回合時，不需要繳過路費。 FIX: 這很奇怪
                AddDomainEvent(new SuspendRoundEvent(CurrentPlayer.Id, CurrentPlayer.SuspendRounds));
                EndRound();
            }
        }
        else
        {
            AddDomainEvent(new EndRoundFailEvent(CurrentPlayer.Id));
        }
    }

    #region Private Functions

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
        Player player = GetPlayer(playerId);
        AddDomainEvent(player.BuyLand(_map, BlockId));
    }

    public void SelectRole(string playerId, string roleId)
    {
        Player player = GetPlayer(playerId);
        player.RoleId = roleId;
        AddDomainEvent(new PlayerSelectRoleEvent(playerId, roleId));
    }

    /// <summary>
    /// 選擇房間位置
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="locationID"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void SelectRoomLocation(string playerId, int locationID)
    {
        Player player = GetPlayer(playerId);
        player.LocationId = locationID;
        AddDomainEvent(new PlaySelectRoomLocationEvent(playerId, player.LocationId));
    }
}