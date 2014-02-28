using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombRush.Networking
{
    public enum GameServerState
    {
        Lobby,
        ProvideDataToClients,
        ProvidedAndWaitForallReady,
        SynchronizeBeforeGameStart,
        InGame,
        MatchResult,
    }
}
