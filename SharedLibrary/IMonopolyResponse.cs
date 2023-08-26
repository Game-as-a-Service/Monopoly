namespace SharedLibrary;

public interface IMonopolyResponses
{
    Task GameCreatedEvent(string gameId);

    Task PlayerJoinGameEvent(string playerId);
    Task PlayerJoinGameFailedEvent(string message);

    Task PlayerRolledDiceEvent(string playerId, int diceCount);

    Task ChessMovedEvent(string playerId, string blockId, string direction, int remainingSteps);

    Task PlayerNeedToChooseDirectionEvent(string playerId, string[] directions);

    Task ThroughStartEvent(string playerId, int gainMoney, decimal totalMoney);

    Task OnStartEvent(string playerId, int gainMoney, decimal totalMoney);

    Task PlayerCanBuildHouseEvent(string playerId, string blockId, int houseCount, decimal upgradeMoney);

    Task PlayerCanBuyLandEvent(string playerId, string blockId, decimal landMoney);

    Task PlayerChooseDirectionEvent(string playerId, string direction);

    Task PlayerCannotMoveEvent(string playerId, int suspendRounds);

    Task PlayerNeedsToPayTollEvent(string playerId, string ownerId, decimal toll);

    Task PlayerBuyBlockEvent(string playerId, string blockId);

    Task PlayerBuyBlockMissedLandEvent(string playerId, string blockId);

    Task PlayerBuyBlockOccupiedByOtherPlayerEvent(string playerId, string blockId);

    Task PlayerBuyBlockInsufficientFundsEvent(string playerId, string blockId, decimal landMoney);

    Task PlayerPayTollEvent(string payerId, decimal payerMoney, string payee, decimal payeeMoney);

    Task PlayerDoesntNeedToPayTollEvent(string payerId, decimal payerMoney);

    Task PlayerTooPoorToPayTollEvent(string payerId, decimal payerMoney, decimal toll);

    Task PlayerBuildHouseEvent(string playerId, string blockId, decimal playerMoney, int house);

    Task PlayerCannotBuildHouseEvent(string PlayerId, string BlockId);

    Task PlayerTooPoorToBuildHouseEvent(string PlayerId, string BlockId, decimal PlayerMoney, decimal UpgradePrice);

    Task HouseMaxEvent(string PlayerId, string BlockId, int House);

    Task PlayerMortgageEvent(string PlayerId, decimal PlayerMoney, string BlockId, int DeadLine);

    Task PlayerCannotMortgageEvent(string PlayerId, decimal PlayerMoney, string BlockId);

    Task MortgageDueEvent(string PlayerId, string BlockId);

    Task MortgageCountdownEvent(string PlayerId, string BlockId, int DeadLine);

    Task PlayerRedeemEvent(string PlayerId, decimal PlayerMoney, string BlockId);

    Task PlayerTooPoorToRedeemEvent(string PlayerId, decimal PlayerMoney, string BlockId, decimal RedeemPrice);

    Task LandNotInMortgageEvent(string PlayerId, string BlockId);

    Task PlayerBidEvent(string PlayerId, string BlockId, decimal HighestPrice);

    Task PlayerBidFailEvent(string PlayerId, string BlockId, decimal BidPrice, decimal HighestPrice);

    Task PlayerTooPoorToBidEvent(string PlayerId, decimal PlayerMoney, decimal BidPrice, decimal HighestPrice);

    Task EndAuctionEvent(string PlayerId, decimal PlayerMoney, string BlockId, string? Owner, decimal OwnerMoney);

    Task EndRoundEvent(string PlayerId, string NextPlayerId);

    Task EndRoundFailEvent(string PlayerId);

    Task SuspendRoundEvent(string PlayerId, int SuspendRounds);
}