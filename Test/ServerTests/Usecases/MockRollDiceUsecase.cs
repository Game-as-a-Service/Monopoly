using Application.Common;
using Application.Usecases;
using Domain.Common;

namespace ServerTests.Usecases;

public class MockRollDiceUsecase : RollDiceUsecase
{
    private readonly MockDiceService _mockDiceService;
    public MockRollDiceUsecase(IRepository repository, IEventBus<DomainEvent> eventBus, MockDiceService mockDiceService)
        : base(repository, eventBus)
    {
        _mockDiceService = mockDiceService;
    }

    public override async Task ExecuteAsync(RollDiceRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        // Mock Dice
        game.Dices = _mockDiceService.Dices;

        //改
        game.PlayerRollDice(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}