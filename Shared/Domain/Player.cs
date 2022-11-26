using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Domain;

public class Player
{
    public PlayerState State { get; private set; } = PlayerState.Normal;

    public void SetState(PlayerState playerState)
    {
        State = playerState;
    }

    public bool IsBankrupt() {
        return State == PlayerState.Bankrupt;
    }
}
