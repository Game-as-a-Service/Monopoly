namespace Server.Hubs;

public interface IMonopolyResponses
{
    Task GameCreatedEvent(string gameId);
    Task PlayerRolledDiceEvent(string playerId, int diceCount);
    Task ChessMovedEvent(string playerId, string blockId, string direction, int remainingSteps);
    Task PlayerNeedToChooseDirectionEvent(string playerId, string[] directions);
    Task ThroughStartEvent(string playerId, int gainMoney,  decimal totalMoney);
    Task OnStartEvent(string playerId, int gainMoney,  decimal totalMoney);
    Task PlayerCanBuildHouseEvent(string playerId, string blockId, int houseCount, decimal upgradeMoney);
    Task PlayerChooseDirectionEvent(string playerId, string direction);
    Task PlayerCannotMoveEvent(string playerId, int suspendRounds);
}