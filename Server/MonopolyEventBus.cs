namespace Server;

public class MonopolyEventBus : IEventBus<DomainEvent>
{
    private readonly IHubContext<MonopolyHub> hubContext;

    public MonopolyEventBus(IHubContext<MonopolyHub> hubContext)
    {
        this.hubContext = hubContext;
    }

    // 這裡暫時採用
    // 1. 不同的event，使用不同的發送方式
    // 2. 傳送到所有使用者
    // 3. 之後要視不同的Event發送到不同的Client。
    //    像是個人的錯誤，只傳送到特定使用者
    //    像是遊戲的狀態，傳送到Group Id為Game Id的所有玩家
    public async Task PublishAsync(IEnumerable<DomainEvent> events)
    {
        foreach (var e in events)
        {
            var type = e.GetType();
            // args is property of e
            var args = type.GetProperties().Select(p => p.GetValue(e)).ToArray();
            // await hubContext1.Clients.Group(gameId).SendCoreAsync(type.Name, args);
            await hubContext.Clients.All.SendCoreAsync(type.Name, args);
        }
    }
}