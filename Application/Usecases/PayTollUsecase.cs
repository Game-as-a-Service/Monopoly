using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record PayTollRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public class PayTollUsecase : Usecase<PayTollRequest>
{
    public PayTollUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(PayTollRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.PayToll(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}