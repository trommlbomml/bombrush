using System;
using Lidgren.Network;

namespace BombRush.Networking
{
    public abstract class Message
    {
        public byte SessionId { get; private set; }
        public double TimeStamp { get; private set; }

        public virtual NetDeliveryMethod DeliveryMethod { get { return NetDeliveryMethod.ReliableOrdered; } }        
        public virtual int SequenceChannel { get { return 0; } }

        protected Message()
        {            
        }

        protected Message(double timeStamp)
        {
            TimeStamp = timeStamp;
        }

        protected virtual void ReadFrom(NetIncomingMessage incomingMessage)
        {
            SessionId = incomingMessage.ReadByte();
            TimeStamp = incomingMessage.ReadDouble();
        }

        protected virtual void WriteTo(NetOutgoingMessage outgoingMessage)
        {
            outgoingMessage.Write(SessionId);
            outgoingMessage.Write(TimeStamp);
        }

        private static readonly object MessageBufferLock = new object();
        private static NetOutgoingMessage _messageBuffer;

        public NetOutgoingMessage Write(MessageTypeMap messageTypeMap, NetPeer peer)
        {
            NetOutgoingMessage outgoingMessage;

            lock (MessageBufferLock)
            {
                if (_messageBuffer == null)
                {
                    _messageBuffer = peer.CreateMessage(128);
                }
                else
                {
                    _messageBuffer.LengthBytes = 0;
                    var buffer = _messageBuffer.PeekDataBuffer();
                    Array.Clear(buffer, 0, buffer.Length);
                }

                var messageTypeId = messageTypeMap.GetMessageTypeId(this);
                _messageBuffer.Write(messageTypeId);

                WriteTo(_messageBuffer);

                outgoingMessage = peer.CreateMessage(_messageBuffer.LengthBytes);
                outgoingMessage.Write(_messageBuffer);
            }

            return outgoingMessage;
        }

        public static Message Read(MessageTypeMap messageTypeMap, NetIncomingMessage incomingMessage)
        {
            var id = incomingMessage.ReadByte();
            var messageType = messageTypeMap.GetMessageType(id);
            var message = (Message)Activator.CreateInstance(messageType);

            message.ReadFrom(incomingMessage);

            return message;
        }

        public void Send(MessageTypeMap messageTypeMap, NetConnection recipient)
        {
            var peer = recipient.Peer;
            var msg = Write(messageTypeMap, peer);
            peer.SendMessage(msg, recipient, DeliveryMethod, SequenceChannel);
        }

        public void SendToAll(MessageTypeMap messageTypeMap, NetServer server, NetConnection except)
        {
            if (server.ConnectionsCount == 0) return;
            var msg = Write(messageTypeMap, server);
            server.SendToAll(msg, except, DeliveryMethod, SequenceChannel);
        }

        public void SendToAll(MessageTypeMap messageTypeMap, NetServer server)
        {
            if (server.ConnectionsCount == 0) return;

            var msg = Write(messageTypeMap, server);
            server.SendToAll(msg, null, DeliveryMethod, SequenceChannel);
        }

    }
}
