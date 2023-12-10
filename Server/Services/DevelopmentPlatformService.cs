using Server.DataModels;

namespace Server.Services;

public class DevelopmentPlatformService : IPlatformService
{
    private readonly IConfiguration _configuration;

    public DevelopmentPlatformService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<UserInfo> GetUserInfo(string tokenString)
    {
        // 根據 tokenString 取得使用者資訊
        var usersSection = _configuration.GetSection("Authentication:Users");
        var users = usersSection.GetChildren();
        foreach (var user in users)
        {
            var token = user["Token"];
            if (token == tokenString)
            {
                var id = user["Id"];
                var email = user["Email"];
                var nickname = user["NickName"];
                var userInfo = new UserInfo(id!, email!, nickname!);
                return Task.FromResult(userInfo);
            }
        }
        throw new Exception("找不到使用者資訊");
    }

    public (string Id, string Token)[] GetUsers()
    {
        var usersSection = _configuration.GetSection("Authentication:Users");
        var users = usersSection.GetChildren();
        var usersInfo = new List<(string Id, string Token)>();
        foreach (var user in users)
        {
            var id = user["Id"];
            var token = user["Token"];
            usersInfo.Add((id!, token!));
        }
        return usersInfo.ToArray();
    }
}
