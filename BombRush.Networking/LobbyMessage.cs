
namespace BombRush.Networking
{
    public struct GameSessionMemberData
    {
        public byte Id;
        public string Name;
        public  bool IsPlayer;
        public int PlayerIndex;
        public int Wins;
    }

    /// <summary>
    /// Eine Nachricht an alle Clients über den Lobbystatus aller verbundenen Spieler.
    /// </summary>
    public class LobbyMessage : Message
    {
        public LobbyMessage(double timeStamp, GameSessionMemberData[] memberData)
            : base(timeStamp)
        {
            Members = memberData;
        }

        public GameSessionMemberData[] Members;
            
        protected override void ReadFrom(Lidgren.Network.NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);
            var length = incomingMessage.ReadInt32(3);
            Members = new GameSessionMemberData[length];
            for (var i = 0; i < length; i++)
            {
                var member = new GameSessionMemberData();
                member.Id = incomingMessage.ReadByte();
                member.Name = incomingMessage.ReadString();
                member.IsPlayer = incomingMessage.ReadBoolean();
                member.PlayerIndex = incomingMessage.ReadInt32(3);
                member.Wins = incomingMessage.ReadInt32(4);
                Members[i] = member;
            }
        }

        protected override void WriteTo(Lidgren.Network.NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);
            outgoingMessage.Write(Members.Length, 3);
            foreach (var member in Members)
            {
                outgoingMessage.Write(member.Id);
                outgoingMessage.Write(member.Name);
                outgoingMessage.Write(member.IsPlayer);
                outgoingMessage.Write(member.PlayerIndex, 3);
                outgoingMessage.Write(member.Wins, 4);
            }
            outgoingMessage.WritePadBits();
        }
    }
}
