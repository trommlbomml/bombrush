
using Lidgren.Network;

namespace BombRush.Networking
{
    public class ClientReadyMessage : ClientMessage
    {
        public bool IsReady { get; private set; }
        public bool LoadForGameReady { get; private set; }

        public ClientReadyMessage()
        {            
        }

        public ClientReadyMessage(double timeStamp, byte clientId, bool isReady, bool loadForGameReady)
            : base(timeStamp, clientId)
        {
            IsReady = isReady;
            LoadForGameReady = loadForGameReady;
        }

        protected override void WriteTo(NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);
            outgoingMessage.Write(IsReady);
            outgoingMessage.Write(LoadForGameReady);
        }

        protected override void ReadFrom(NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);
            IsReady = incomingMessage.ReadBoolean();
            LoadForGameReady = incomingMessage.ReadBoolean();
        }
    }
}
