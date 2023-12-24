using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record PayTollRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public record PayTollResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PayTollUsecase(IRepository repository)
    : Usecase<PayTollRequest, PayTollResponse>(repository)
{
    public override async Task ExecuteAsync(PayTollRequest request, IPresenter<PayTollResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.PayToll(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new PayTollResponse(game.DomainEvents));
    }
}