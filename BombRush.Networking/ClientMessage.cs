
namespace BombRush.Networking
{
    public abstract class ClientMessage : Message
    {
        protected ClientMessage()
        {
        }

        protected ClientMessage(double timeStamp, byte clientId)
            : base(timeStamp)
        {
            ClientId = clientId;
        }

        public byte ClientId { get; private set; }

        protected override void ReadFrom(Lidgren.Network.NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);
            ClientId = incomingMessage.ReadByte();
        }

        protected override void WriteTo(Lidgren.Network.NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);
            outgoingMessage.Write(ClientId);
        }
    }
}
