using System;
using System.Collections.ObjectModel;
using System.Net;
using BombRush.Interfaces;
using Lidgren.Network;

namespace BombRush.Network
{
    class RemoteGameCreationSession : GameCreationSession
    {
        private NetClient _netClient;

        public RemoteGameCreationSession(IPAddress address)
        {
            State = GameCreationSessionState.ConnectingToServer;
        }

        public GameCreationSessionState State { get; private set; }
        public GameSessionMember[] Members { get; private set; }
        public ReadOnlyCollection<GameInstance> RunningGameInstances { get; private set; }
        public bool IsAdministrator { get; private set; }

        public void CreateGameInstance(string gameName, string playerName)
        {
            if (State != GameCreationSessionState.GameLobby) throw new InvalidOperationException("Cann create game only in state GameLobby");
        }

        public void JoinGameInstance(GameInstance instance, string playerName)
        {
            if (State != GameCreationSessionState.GameLobby) throw new InvalidOperationException("Cann create game only in state GameLobby");


        }

        public void LeaveGameInstance()
        {
            throw new NotImplementedException();
        }
    }
}
