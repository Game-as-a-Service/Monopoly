using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Server.DataModels;
using System.Net;
using System.Net.Http.Headers;

namespace Server.Services;

public class PlatformService : IPlatformService
{
    private readonly JwtBearerOptions _jwtOptions;

    public PlatformService(IOptionsMonitor<JwtBearerOptions> options)
    {
        _jwtOptions = options.Get("Bearer");
    }

    public async Task<UserInfo> GetUserInfo(string tokenString)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_jwtOptions.Audience!)
        };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);
        var response = await httpClient.GetAsync("/users/me");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new BadHttpRequestException($"Get user info failed: {response.StatusCode}");
        }
        var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();

        return userInfo!;
    }
}

public interface IPlatformService
{
    public Task<UserInfo> GetUserInfo(string tokenString);
}
