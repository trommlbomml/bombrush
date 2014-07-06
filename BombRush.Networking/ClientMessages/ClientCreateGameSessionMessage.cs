
using System;
using Lidgren.Network;

namespace BombRush.Networking.ClientMessages
{
    public class ClientCreateGameSessionMessage : ClientMessageBase
    {
        public string UserName { get; private set; }
        public string GameName { get; private set; }
        public Int16 MatchTime { get; set; }
        public byte MatchesToWin { get; set; }

        public ClientCreateGameSessionMessage(string userName, string gameName, Int16 matchTime, byte matchesToWin)
        {
            UserName = userName;
            GameName = gameName;
            MatchTime = matchTime;
            MatchesToWin = matchesToWin;
        }

        protected override void ReadFrom(NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);
            UserName = incomingMessage.ReadString();
            GameName = incomingMessage.ReadString();
            MatchTime = incomingMessage.ReadInt16();
            MatchesToWin = incomingMessage.ReadByte();
        }

        protected override void WriteTo(NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);
            outgoingMessage.Write(UserName);
            outgoingMessage.Write(GameName);
            outgoingMessage.Write(MatchTime);
            outgoingMessage.Write(MatchesToWin);
        }
    }
}
