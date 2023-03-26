﻿namespace Server.Hubs;

public interface IMonopolyResponses
{
    Task GameCreatedEvent(string gameId);
    Task PlayerRolledDiceEvent(string playerId, int diceCount);
    Task ChessMovedEvent(string playerId, string blockId, string direction, int remainingSteps); 
}