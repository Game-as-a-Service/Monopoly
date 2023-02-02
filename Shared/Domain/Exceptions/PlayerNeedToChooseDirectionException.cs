using System.Runtime.Serialization;

namespace Shared.Domain.Exceptions;

[Serializable]
public class PlayerNeedToChooseDirectionException : Exception
{
    public PlayerNeedToChooseDirectionException()
    {
    }

    public PlayerNeedToChooseDirectionException(string? message) : base(message)
    {
    }

    public PlayerNeedToChooseDirectionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public PlayerNeedToChooseDirectionException(Player player, Block currentBlock, List<Map.Direction> directions)
    {
        // TODO: 把這裡的訊息寫好
    }

    protected PlayerNeedToChooseDirectionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}