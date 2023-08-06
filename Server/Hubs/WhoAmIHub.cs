using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

[Authorize]
public class WhoAmIHub: Hub
{
    public async Task WhoAmI()
    {
        // JWT 的 claims should be in principal
        var principal = Context.User;
        var claims = principal!.Claims.Select(x => $"{x.Type}: {x.Value}").ToList();
        await Clients.Caller.SendAsync("WhoAmI", claims);
    }
}
