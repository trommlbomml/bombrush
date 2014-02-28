using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;

namespace BombRush.Networking
{
    public class RefreshClientListMessage : Message
    {
        public struct ClientData
        {
            public byte ClientId;
            public string Name;
            public bool IsReady;

            public ClientData(byte clientId, string name, bool isReady)
            {
                ClientId = clientId;
                Name = name;
                IsReady = isReady;
            }
        }

        public override NetDeliveryMethod DeliveryMethod
        {
            get { return NetDeliveryMethod.Unreliable; }
        }

        public RefreshClientListMessage()
        {            
        }

        public RefreshClientListMessage(double timeStamp, IEnumerable<ClientData> clients)
            : base(timeStamp)
        {
            _clients = clients.ToArray();
        }

        private ClientData[] _clients;
        public IEnumerable<ClientData> Clients { get { return _clients; } }

        protected override void ReadFrom(NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);
            var clientCount = incomingMessage.ReadByte();
            _clients = new ClientData[clientCount];
            for(var i = 0; i < clientCount; i++)
            {
                _clients[i].ClientId = incomingMessage.ReadByte();
                _clients[i].Name = incomingMessage.ReadString();
                _clients[i].IsReady = incomingMessage.ReadBoolean();
            }
        }

        protected override void WriteTo(NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);
            outgoingMessage.Write((byte)_clients.Length);
            foreach(var client in _clients)
            {
                outgoingMessage.Write(client.ClientId);
                outgoingMessage.Write(client.Name);
                outgoingMessage.Write(client.IsReady);
            }
        }
    }
}
