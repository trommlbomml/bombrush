using System;
using Game2DFramework;
using Lidgren.Network;

namespace BombRush.Networking.Extensions
{
    public static class NetPeerExtension
    {
        public static void HandleNetMessages(this NetClient netClient, double timeStamp, Action<double, NetIncomingMessage> handleDataMessage, Action<double, NetIncomingMessage> handleDiscoveryResponse)
        {
            NetIncomingMessage inc;
            while ((inc = netClient.ReadMessage()) != null)
            {
                HandleCommonMessageTypes(timeStamp, inc, handleDataMessage);
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        handleDiscoveryResponse(timeStamp, inc);
                        break;
                }
            }
        }

        public static void HandleNetMessages(this NetClient netClient, double timeStamp, Action<double, NetIncomingMessage> handleDataMessage, Action<double, NetConnectionStatus, string> handleStatusChanged)
        {
            NetIncomingMessage inc;
            while ((inc = netClient.ReadMessage()) != null)
            {
                HandleCommonMessageTypes(timeStamp, inc, handleDataMessage);
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        var netStatus = (NetConnectionStatus) inc.ReadByte();
                        var reason = inc.ReadString();
                        handleStatusChanged(timeStamp, netStatus, reason);
                        break;
                }
            }
        }

        public static void HandleNetMessages(this NetServer netServer, double timeStamp, Action<double, NetIncomingMessage> handleDataMessage, Action<double, NetIncomingMessage> handleStatusChangedMessage, Action<double, NetIncomingMessage> handleConnectionApproval)
        {
            NetIncomingMessage inc;
            while ((inc = netServer.ReadMessage()) != null)
            {
                HandleCommonMessageTypes(timeStamp, inc, handleDataMessage);
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        handleConnectionApproval(timeStamp, inc);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        handleStatusChangedMessage(timeStamp, inc);
                        break;
                }
            }
        }

        private static void HandleCommonMessageTypes(double timeStamp, NetIncomingMessage inc, Action<double, NetIncomingMessage> handleDataMessage)
        {
            switch (inc.MessageType)
            {
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                    Log.Write(string.Format("Network-{0}: {1}",inc.MessageType, inc.ReadString()));
                    break;
                case NetIncomingMessageType.Data:
                    handleDataMessage(timeStamp, inc);
                    break;
            }
        }
    }
}