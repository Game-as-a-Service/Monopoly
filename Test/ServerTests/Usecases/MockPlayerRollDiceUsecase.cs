using Application.Common;
using Application.Usecases;
using Domain.Common;

namespace ServerTests.Usecases;

public class MockPlayerRollDiceUsecase : PlayerRollDiceUsecase
{
    private readonly MockDiceService _mockDiceService;
    public MockPlayerRollDiceUsecase(IRepository repository, MockDiceService mockDiceService)
        : base(repository)
    {
        _mockDiceService = mockDiceService;
    }

    public override async Task ExecuteAsync(PlayerRollDiceRequest request, IPresenter<PlayerRollDiceResponse> presenter)
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
        await presenter.PresentAsync(new PlayerRollDiceResponse(game.DomainEvents));
    }
}