
namespace BombRush.Networking
{
    public class RemainingSynchronizeTimeMessage : Message
    {
        public float RemainingTime { get; private set; }

        public RemainingSynchronizeTimeMessage()
        {
            
        }

        public RemainingSynchronizeTimeMessage(double timeStamp, float remainingTime) :
            base(timeStamp)
        {
            RemainingTime = remainingTime;
        }

        protected override void ReadFrom(Lidgren.Network.NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);

            RemainingTime = incomingMessage.ReadFloat();
        }

        protected override void WriteTo(Lidgren.Network.NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);

            outgoingMessage.Write(RemainingTime);
        }
    }
}
