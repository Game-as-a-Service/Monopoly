using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Server.DataModels;
using Server.Hubs;
using Server.Services;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

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

    public async Task<VerificationHub> CreateHubConnectionAsync(string gameId, string playerId)
    {
        var uri = new UriBuilder(Client.BaseAddress!)
        {
            Path = $"/monopoly",
            Query = $"gameid={gameId}"
        }.Uri;
        var hub = new HubConnectionBuilder()
            .WithUrl(uri, opt =>
            {
                opt.AccessTokenProvider = async () =>
                {
                    var options = GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();
                    
                    var jwtToken = GetRequiredService<MockJwtTokenService>()
                        .GenerateJwtToken(options.Get("Bearer").Audience, playerId);
                    return await Task.FromResult(jwtToken);
                };
                opt.HttpMessageHandlerFactory = _ => Server.CreateHandler();                
            })
            .Build();
        VerificationHub verificationHub = new(hub);
        await hub.StartAsync();

        return verificationHub;
    }


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<MockJwtTokenService>();
            services.RemoveAll<IPlatformService>();
            services.RemoveAll<IOptions<JwtBearerOptions>>();

            services.AddSingleton<IPlatformService, MockPlatformService>();
            services.AddOptions<JwtBearerOptions>("Bearer")
                .Configure<MockJwtTokenService>((options, jwtToken) =>
                {
                    var config = new OpenIdConnectConfiguration()
                    {
                        Issuer = jwtToken.Issuer
                    };

                    config.SigningKeys.Add(jwtToken.SecurityKey);
                    options.Configuration = config;
                    options.Authority = null;
                });
        });
    }
}

internal class VerificationHub
{
    private readonly HubConnection Connection;

    private readonly Dictionary<string, ConcurrentQueue<object[]>> Queues;

    public VerificationHub(HubConnection Connection)
    {
        this.Connection = Connection;
        Queues = new();
        ListenAllEvent();
    }

    // 利用反射讀出所有HubResponse的Method，並且設置On function
    private void ListenAllEvent()
    {
        Type interfaceType = typeof(IMonopolyResponses);
        MethodInfo[] methods = interfaceType.GetMethods();

        foreach (MethodInfo method in methods)
        {
            ParameterInfo[] parameters = method.GetParameters();
            Queues.Add(method.Name, new());

            Type[] parameterTypes = parameters.Select(x => x.ParameterType).ToArray();
            void handler(object?[] x) => Queues[method.Name].Enqueue(x!);
            Connection.On(method.Name, parameterTypes, handler);
        }
    }

    private void Verify(string methodName, Func<object[], bool> verify, int timeout)
    {
        try
        {
            var startTime = DateTime.Now;
            while (true)
            {
                if (Queues[methodName].TryDequeue(out var result))
                {
                    var options = new JsonSerializerOptions()
                    {
                        WriteIndented = true,
                    };

                    // 這裡應該要把 result(object[])轉成T，但目前只有一個參數
                    Assert.IsTrue(verify(result),
                        $"\n回傳結果為 {JsonSerializer.Serialize(result, options)}");
                    break;
                }
                else
                {
                    // 如果已經斷開連線測試失敗
                    if (Connection.State == HubConnectionState.Disconnected)
                    {
                        if(Queues[nameof(IMonopolyResponses.PlayerJoinGameFailedEvent)].TryPeek(out var errorMessages))
                            Assert.Fail(
                                $"""
                                已經斷開連線
                                訊息:
                                {string.Join("\n", errorMessages!)}
                                """);
                    }
                    // 計算已經等待的時間
                    var elapsedMilliseconds = (DateTime.Now - startTime).TotalMilliseconds;
                    if (elapsedMilliseconds >= timeout)
                    {
                        Assert.Fail(                                                                   
                            $"""
                            超出預期時間 {timeout} ms，預期得到 Event【{methodName}】
                            可以嘗試檢查下面的問題:
                            1. 在 EventBus 中，缺少 Event 的傳送
                            2. 在 Usecase 中，沒有使用 EventBus.PublishAsync
                            3. 在 Domain 中，沒有 添加 Domain Event
                            """);
                    }
                    // 等待一段時間再繼續嘗試
                    SpinWait.SpinUntil(() => false, 50);
                }
            }
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
            Assert.Fail(
                $"""

                IMonopolyResponses 中缺少 Method【{methodName}】
                可以在 IMonopolyResponses 添加 【{methodName}】 以解決這個問題
                """);
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine(ex.Message);
            Assert.Fail(
                $"""

                錯誤的轉型
                可能是【IMonopolyResponses Method的參數類型】與【驗證的參數類型】不一樣
                """);
        }
    }

    public async Task SendAsync(string method, params object?[] args)
    {
        await Connection.SendCoreAsync(method, args);
    }

    public void Verify<T1>(string methodName, Func<T1, bool> verify, int timeout = 5000)
    {
        Verify(methodName, args => verify((T1)args[0]), timeout);
    }

    public void Verify<T1, T2>(string methodName, Func<T1, T2, bool> verify, int timeout = 5000)
    {
        Verify(methodName, args => verify((T1)args[0], (T2)args[1]), timeout);
    }

    public void Verify<T1, T2, T3>(string methodName, Func<T1, T2, T3, bool> verify, int timeout = 5000)
    {
        Verify(methodName, args => verify((T1)args[0], (T2)args[1], (T3)args[2]), timeout);
    }

    public void Verify<T1, T2, T3, T4>(string methodName, Func<T1, T2, T3, T4, bool> verify, int timeout = 5000)
    {
        Verify(methodName, args => verify((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]), timeout);
    }

    public void Verify<T1, T2, T3, T4, T5>(string methodName, Func<T1, T2, T3, T4, T5, bool> verify, int timeout = 5000)
    {
        Verify(methodName, args => verify((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4]), timeout);
    }

    // 確認所有的Queue已經是空的了
    public void VerifyNoElseEvent()
    {
        foreach (var (method, queue) in Queues)
        {
            if (method == nameof(IMonopolyResponses.PlayerJoinGameEvent))
            {
                continue;
            }
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            Assert.IsTrue(queue.IsEmpty,
                $"""

                【{method}】中還有 {queue.Count} 筆資料
                {JsonSerializer.Serialize(queue, options)}
                """);
        }
    }
}

internal static class TestHubExtension
{
    public static IDisposable On(this HubConnection hubConnection, string methodName, Type[] parameterTypes, Action<object?[]> handler)
    {
        return hubConnection.On(methodName, parameterTypes, static (parameters, state) =>
        {
            var currentHandler = (Action<object?[]>)state;
            currentHandler(parameters);
            return Task.CompletedTask;
        }, handler);
    }
}

internal class MockJwtTokenService
{
    public string Issuer { get; }
    public SecurityKey SecurityKey { get; }

    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
    private static readonly byte[] _key = new byte[64];

    public MockJwtTokenService()
    {
        Issuer = Guid.NewGuid().ToString();

        _rng.GetBytes(_key);
        SecurityKey = new SymmetricSecurityKey(_key) { KeyId = Guid.NewGuid().ToString() };
        _signingCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateJwtToken(string? audience, string playerId)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = audience,
            Expires = DateTime.UtcNow.AddMinutes(20),
            SigningCredentials = _signingCredentials,
            Subject = new ClaimsIdentity(new Claim[] { new("Id", playerId) }),
        };
        var token = _tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }
}

public class MockPlatformService : IPlatformService
{
    public Task<UserInfo> GetUserInfo(string tokenString)
    {
        var jwt = new JwtSecurityToken(tokenString);

        var id = jwt.Claims.First(x => x.Type == "Id").Value;

        var userinfo = new UserInfo(id, "", "");

        return Task.FromResult(userinfo);
    }
}