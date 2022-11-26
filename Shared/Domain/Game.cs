using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Domain;

public class Game
{
    // 
    Dictionary<string, Player> players = new();


    public void AddPlayer(string id)
    {
        players.Add(id, new Player());
    }

    public void SetState(string id, PlayerState playerState)
    {
        // 不會做事
        FindPlayerById(id)?.SetState(playerState);
    }

    public Player? Settlement()
    {
        // 跟我剛才寫的一樣
        // 先filter，再儲存，然後長度為一就是結束，最後把player從那個長度唯一的變數取出來
        // 感覺我是專門提供想法的XD
        var notBankruptPlayer = players.Where(p => !p.Value.IsBankrupt());
        if (notBankruptPlayer.Count() == 1) {
            return notBankruptPlayer.First().Value;
        }
        // 回傳沒破產的很奇怪，因為還有其他的就代表還沒結束
        // 但 null 很危險
        return null;
    }

    // unsafe
    public Player? FindPlayerById(string id)
    {
        Player? player;
        players.TryGetValue(id, out player); // return bool

        return player;
    }
}
