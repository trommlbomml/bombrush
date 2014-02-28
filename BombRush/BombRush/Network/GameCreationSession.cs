
using System.Collections.ObjectModel;
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
        ConnectingToServer,
        GameLobby,
        JoinedGame,
        FirstMatchStarted,
        ConnectionFailed
    }

    interface GameCreationSession
    {
        GameCreationSessionState State { get; }
        GameSessionMember[] Members { get; }
        ReadOnlyCollection<GameInstance> RunningGameInstances { get; }
        bool IsAdministrator { get; }

        void CreateGameInstance(string gameName, string playerName);
        void JoinGameInstance(GameInstance instance, string playerName);
        void LeaveGameInstance();
    }
}
