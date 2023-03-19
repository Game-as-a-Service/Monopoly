using Domain.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace ServerTests;

internal class MonopolyTestServer : WebApplicationFactory<Program>
{
    public HttpClient Client { get; init; }

    public MonopolyTestServer()
    {
        Client = CreateClient();
    }

    public T GetRequiredService<T>()
            where T : notnull
    {
        return Server.Services.CreateScope().ServiceProvider.GetRequiredService<T>();
    }

    public HubConnection CreateHubConnection()
    {
        var uri = new UriBuilder(Client.BaseAddress!)
        {
            Path = "/monopoly"
        }.Uri;
        var hub = new HubConnectionBuilder()
            .WithUrl(uri, opt =>
            {
                opt.HttpMessageHandlerFactory = _ => Server.CreateHandler();
            })
            .Build();
        hub.StartAsync();
        return hub;
    }
}

internal static class HubConnectionExtensions
{
    public static HubVerification<T> Verify<T>(this HubConnection Connection, string MethodName, int Timeout = 10000)
            where T : DomainEvent
    {
        TaskCompletionSource<T> source = new();

        int timeoutMs = Timeout;
        var ct = new CancellationTokenSource(timeoutMs);
        ct.Token.Register(() => source.TrySetCanceled(), useSynchronizationContext: false);

        Connection.On<T>(MethodName, source.SetResult);
        return new HubVerification<T>(source);
    }
}

internal class HubVerification<T>
{
    private readonly TaskCompletionSource<T> source;

    public HubVerification(TaskCompletionSource<T> source)
    {
        this.source = source;
    }

    public async Task Verify(Func<T, bool> VerifyAction)
    {
        var e = await source.Task;
        Assert.IsTrue(VerifyAction(e));
    }
}