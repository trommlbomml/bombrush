
using System.Collections.ObjectModel;
using BombRush.Interfaces;

namespace BombRush.Network
{
    interface GameCreationSession
    {
        GameCreationSessionState State { get; }
        ReadOnlyCollection<GameInstance> RunningGameInstances { get; }

        void ConnectToServer(string host, string playerName);
        GameSession CreateGameInstance(string gameName);
        GameSession JoinGameInstance(GameInstance instance);
        void LeaveGameInstance();
    }
}
