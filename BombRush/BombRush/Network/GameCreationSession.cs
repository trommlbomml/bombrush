
using System.Collections.ObjectModel;
using System.Net;
using BombRush.Interfaces;

namespace BombRush.Network
{
    class GameInstance
    {
        public byte SessionId { get; set; }
        public string Name { get; set; }
        public int PlayerCount { get; set; }
        public bool IsRunning { get; set; }
    }

    enum GameCreationSessionState
    {
        Disconnected,
        ConnectingToServer,
        Connected,
        ConnectionToServerFailed
    }

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
