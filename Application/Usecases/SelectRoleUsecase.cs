using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record SelectRoleRequest(string GameId, string PlayerId, string roleId)
    : Request(GameId, PlayerId);

public record SelectRoleResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class SelectRoleUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<SelectRoleRequest, SelectRoleResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(SelectRoleRequest request, IPresenter<SelectRoleResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();
        //改
        game.SelectRole(request.PlayerId, request.roleId);
        //存
        Repository.Save(game);
        //推
        await presenter.PresentAsync(new SelectRoleResponse(game.DomainEvents));
    }
}
