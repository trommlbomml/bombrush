
using Lidgren.Network;

namespace BombRush.Networking.ServerMessages
{
    public class GameCreationStatusMessage : Message
    {
        public override NetDeliveryMethod DeliveryMethod
        {
            get { return NetDeliveryMethod.UnreliableSequenced; }
        }

        public GameInstanceData[] Instances { get; private set; }

        public GameCreationStatusMessage(GameInstanceData[] instances)
        {
            Instances = instances;
        }

        protected override void ReadFrom(NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);
            
            var count = incomingMessage.ReadInt16();
            Instances = new GameInstanceData[count];

            for (var i = 0; i < count; i++)
            {
                Instances[i] = new GameInstanceData
                {
                    SessionId = incomingMessage.ReadByte(),
                    Name = incomingMessage.ReadString(),
                    PlayerCount = incomingMessage.ReadByte(4),
                    IsRunning = incomingMessage.ReadBoolean()
                };
            }
        }

        protected override void WriteTo(NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);
            outgoingMessage.Write((short)Instances.Length);
            foreach (var gameInstanceData in Instances)
            {
                outgoingMessage.Write(gameInstanceData.SessionId);
                outgoingMessage.Write(gameInstanceData.Name);
                outgoingMessage.Write(gameInstanceData.PlayerCount, 4);
                outgoingMessage.Write(gameInstanceData.IsRunning);
            }
            outgoingMessage.WritePadBits();
        }
    }
}
