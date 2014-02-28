using System.Collections.Generic;
using System.Linq;
using BombRush.Interfaces;

namespace BombRush.Networking
{
    public struct MatchPlayerInformation
    {
        public string ClientName;
        public byte Wins;
        public byte Id;
        public bool IsMatchWinner;

        public MatchPlayerInformation(ClientInformation c) : this(c.Id, c.Name, (byte)c.Figure.Wins, c.Figure.IsMatchWinner)
        {
            
        }

        public MatchPlayerInformation(byte id, string name, byte wins, bool isMatchWinner)
        {
            Id = id;
            ClientName = name;
            Wins = wins;
            IsMatchWinner = isMatchWinner;
        }
    }

    public class MatchFinishedMessage : Message
    {
        public MatchFinishedMessage()
        {
            
        }

        public MatchFinishedMessage(double timeStamp, IList<ClientInformation> clientInformation, MatchResultType resultType) : base(timeStamp)
        {
            ResultType = resultType;
            int i = 0;
            Players = new MatchPlayerInformation[clientInformation.Count()];
            foreach (ClientInformation information in clientInformation)
            {
                Players[i++] = new MatchPlayerInformation(information);
            }
        }

        public MatchPlayerInformation[] Players { get; private set; }
        public MatchResultType ResultType { get; private set; }

        protected override void ReadFrom(Lidgren.Network.NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);
            ResultType = (MatchResultType)incomingMessage.ReadByte();
            int playerCount = incomingMessage.ReadByte();
            Players = new MatchPlayerInformation[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                byte id = incomingMessage.ReadByte();
                string name = incomingMessage.ReadString();
                byte wins = incomingMessage.ReadByte(7);
                bool isMatchWinner = incomingMessage.ReadBoolean();

                Players[i] = new MatchPlayerInformation(id, name, wins, isMatchWinner);
            }
        }

        protected override void WriteTo(Lidgren.Network.NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);
            outgoingMessage.Write((byte)ResultType);
            outgoingMessage.Write((byte)Players.Length);
            foreach (MatchPlayerInformation matchPlayerInformation in Players)
            {
                outgoingMessage.Write(matchPlayerInformation.Id);
                outgoingMessage.Write(matchPlayerInformation.ClientName);
                outgoingMessage.Write(matchPlayerInformation.Wins, 7);
                outgoingMessage.Write(matchPlayerInformation.IsMatchWinner);
            }
        }
    }
}
