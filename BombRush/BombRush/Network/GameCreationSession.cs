
using System.Collections.ObjectModel;
using BombRush.Interfaces;

namespace BombRush.Network
{
    interface GameCreationSession
    {
        GameCreationSessionState State { get; }
        ReadOnlyCollection<GameInstance> RunningGameInstances { get; }

        void ConnectToServer(string host);
        GameSession CreateGameInstance(string gameName, string playerName);
        GameSession JoinGameInstance(GameInstance instance, string playerName);
        void LeaveGameInstance();
    }
}
