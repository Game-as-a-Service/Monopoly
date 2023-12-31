using SharedLibrary.ResponseArgs.Monopoly;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace SharedLibrary;

public interface IMonopolyResponses
{
    Task PlayerJoinGameEvent(PlayerJoinGameEventArgs e);
    Task PlayerJoinGameFailedEvent(PlayerJoinGameFailedEventArgs e);

    Task PlayerRolledDiceEvent(PlayerRolledDiceEventArgs e);

    Task ChessMovedEvent(ChessMovedEventArgs e);

    Task PlayerNeedToChooseDirectionEvent(PlayerNeedToChooseDirectionEventArgs e);

    Task ThroughStartEvent(PlayerThroughStartEventArgs e);

    Task CannotGetRewardBecauseStandOnStartEvent(CannotGetRewardBecauseStandOnStartEventArgs e);

    Task PlayerCanBuildHouseEvent(PlayerCanBuildHouseEventArgs e);

    Task PlayerCanBuyLandEvent(PlayerCanBuyLandEventArgs e);

    Task PlayerChooseDirectionEvent(PlayerChooseDirectionEventArgs e);

    Task PlayerNeedsToPayTollEvent(PlayerNeedsToPayTollEventArgs e);

    Task PlayerBuyBlockEvent(PlayerBuyBlockEventArgs e);

    Task PlayerBuyBlockOccupiedByOtherPlayerEvent(PlayerBuyBlockOccupiedByOtherPlayerEventArgs e);

    Task PlayerBuyBlockInsufficientFundsEvent(PlayerBuyBlockInsufficientFundsEventArgs e);

    Task PlayerPayTollEvent(PlayerPayTollEventArgs e);

    Task PlayerDoesntNeedToPayTollEvent(PlayerDoesntNeedToPayTollEventArgs e);

    Task PlayerTooPoorToPayTollEvent(PlayerTooPoorToPayTollEventArgs e);

    Task PlayerBuildHouseEvent(PlayerBuildHouseEventArgs e);

    Task PlayerCannotBuildHouseEvent(PlayerCannotBuildHouseEventArgs e);

    Task HouseMaxEvent(HouseMaxEventArgs e);

    Task PlayerMortgageEvent(PlayerMortgageEventArgs e);

    Task PlayerCannotMortgageEvent(PlayerCannotMortgageEventArgs e);

    Task MortgageDueEvent(MortgageDueEventArgs e);
    
    Task PlayerRedeemEvent(PlayerRedeemEventArgs e);

    Task PlayerTooPoorToRedeemEvent(PlayerTooPoorToRedeemEventArgs e);
    
    Task PlayerBidEvent(PlayerBidEventArgs e);

    Task PlayerBidFailEvent(PlayerBidFailEventArgs e);

    Task PlayerTooPoorToBidEvent(PlayerTooPoorToBidEventArgs e);

    Task CurrentPlayerCannotBidEvent(CurrentPlayerCannotBidEventArgs e);

    Task EndAuctionEvent(EndAuctionEventArgs e);

    Task EndRoundEvent(EndRoundEventArgs e);

    Task EndRoundFailEvent(EndRoundFailEventArgs e);

    Task SuspendRoundEvent(SuspendRoundEventArgs e);

    Task PlayerBankruptEvent(PlayerBankruptEvent e);

    Task SettlementEvent(SettlementEventArgs e);

    Task PlayerSelectRoleEvent(PlayerSelectRoleEventArgs e);

    Task PlaySelectLocationEvent(PlaySelectLocationEventArgs e);

    Task PlayCannotSelectLocationEvent(PlayCannotSelectLocationEventArgs e);

    Task PlayerReadyEvent(PlayerReadyEventArgs e);

    Task PlayerCannotReadyEvent(PlayerCannotReadyEventArgs e);

    Task GameStartEvent(GameStartEventArgs e);

    Task OnlyOnePersonEvent(OnlyOnePersonEventArgs e);

    Task SomePlayersPreparingEvent(SomePlayersPreparingEventArgs e);
    Task GetReadyInfoEvent(GetReadyInfoEventArgs e);
    Task WelcomeEvent(WelcomeEventArgs e);
}