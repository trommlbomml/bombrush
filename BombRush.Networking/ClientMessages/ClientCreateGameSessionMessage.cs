
using Lidgren.Network;

namespace BombRush.Networking.ClientMessages
{
    public class ClientCreateGameSessionMessage : ClientMessageBase
    {
        public string UserName { get; private set; }
        public string GameName { get; private set; }

        public ClientCreateGameSessionMessage(string userName, string gameName)
        {
            UserName = userName;
            GameName = gameName;
        }

        protected override void ReadFrom(NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);
            UserName = incomingMessage.ReadString();
            GameName = incomingMessage.ReadString();
        }

        protected override void WriteTo(NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);
            outgoingMessage.Write(UserName);
            outgoingMessage.Write(GameName);
        }
    }
}
