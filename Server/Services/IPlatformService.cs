using Server.DataModels;

namespace Server.Services;

public interface IPlatformService
{
    public Task<UserInfo> GetUserInfo(string tokenString);
}