
using Lidgren.Network;

namespace BombRush.Networking
{
    public enum ConnectionInformation
    {
        ConnectedWithId,
        NoUniqueName,
    }

    public class ConnectionStatusMessage : ClientMessage
    {
        public ConnectionInformation ConnectionInformation { get; private set; }

        public ConnectionStatusMessage()
        {
        }

        public ConnectionStatusMessage(double timeStamp, byte clientId, ConnectionInformation information)
            : base(timeStamp, clientId)
        {
            ConnectionInformation = information;
        }

        protected override void WriteTo(NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);
            outgoingMessage.Write((byte)ConnectionInformation);
        }

        protected override void ReadFrom(NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);
            ConnectionInformation = (ConnectionInformation) incomingMessage.ReadByte();
        }
    }
}
