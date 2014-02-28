using System;
using System.Collections.Generic;

namespace BombRush.Networking
{
    public class MessageTypeMap
    {
        private readonly Dictionary<Type, byte> _messageTypeIds = new Dictionary<Type, byte>();
        private readonly Dictionary<byte, Type> _messageTypes = new Dictionary<byte, Type>();

        public MessageTypeMap()
        {
            RegisterType(typeof(ClientReadyMessage));
            RegisterType(typeof(ConnectionStatusMessage));
            RegisterType(typeof(GameDataTransferMessage));
            RegisterType(typeof(GameStatusMessage));
            RegisterType(typeof(InputMessage));
            RegisterType(typeof(MatchFinishedMessage));
            RegisterType(typeof(RefreshClientListMessage));
            RegisterType(typeof(RemainingSynchronizeTimeMessage));
            RegisterType(typeof(StartGameMessage));
            RegisterType(typeof(SwitchToReceiveDataMessage));
        }

        private void RegisterType(Type type)
        {
            var id = (byte) (_messageTypeIds.Count + 1);
            _messageTypeIds.Add(type, id);
            _messageTypes.Add(id, type);
        }

        public byte GetMessageTypeId(Message message)
        {
            return _messageTypeIds[message.GetType()];
        }

        public Type GetMessageType(byte messageTypeId)
        {
            return _messageTypes[messageTypeId];
        }
    }
}
