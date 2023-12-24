using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record ChooseDirectionRequest(string GameId, string PlayerId, string Direction)
    : Request(GameId, PlayerId);

public record ChooseDirectionResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class ChooseDirectionUsecase(IRepository repository)
    : Usecase<ChooseDirectionRequest, ChooseDirectionResponse>(repository)
{
    public override async Task ExecuteAsync(ChooseDirectionRequest request, IPresenter<ChooseDirectionResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();
        //改
        game.PlayerSelectDirection(request.PlayerId, request.Direction);
        //存
        Repository.Save(game);
        //推
        await presenter.PresentAsync(new ChooseDirectionResponse(game.DomainEvents));
    }
}