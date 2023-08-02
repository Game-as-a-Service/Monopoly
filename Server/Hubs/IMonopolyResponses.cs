﻿namespace Server.Hubs;

public interface IMonopolyResponses
{
    Task GameCreatedEvent(string gameId);

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
}