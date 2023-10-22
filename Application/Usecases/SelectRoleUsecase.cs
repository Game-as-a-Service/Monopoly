using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record SelectRoleRequest(string GameId, string PlayerId, string roleId)
    : Request(GameId, PlayerId);

public class SelectRoleUsecase : Usecase<SelectRoleRequest>
{
    public SelectRoleUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }
    public override async Task ExecuteAsync(SelectRoleRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();
        //改
        game.SelectRole(request.PlayerId, request.roleId);
        //存
        Repository.Save(game);
        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}
