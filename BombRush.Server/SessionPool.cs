
using System.Collections.Generic;
using System.Linq;

namespace BombRush.Server
{
    class SessionPool
    {
        private readonly List<Session> _allSessions = new List<Session>();

        public SessionPool(int maxSessionCount)
        {
            for (byte i = 1; i <= maxSessionCount; i++)
            {
                _allSessions.Add(new Session(i));
            }
        }

        private Session GetNextDeactivatedSession()
        {
            return _allSessions.First();
        }

        public bool ActivateSession(GameClient client, string sessionName)
        {
            var session = GetNextDeactivatedSession();
            if (session == null) return false;

            client.Session = session;
            client.IsSessionAdministrator = true;
            session.JoinClient(client);

            return true;
        }

        public void HandleClientLeft(GameClient client)
        {
            _allSessions.ForEach(s => s.HandleClientLeft(client));
        }

        public bool JoinSession(GameClient client, byte sessionId)
        {
            var session = _allSessions.FirstOrDefault(s => s.Id == sessionId);
            if (session != null)
            {
                client.Session = session;
                client.IsSessionAdministrator = false;
                session.JoinClient(client);
                return true;
            }

            return false;
        }
    }
}
