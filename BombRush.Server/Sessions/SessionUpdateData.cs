using BombRush.Networking;

namespace BombRush.Server.Sessions
{
    class SessionUpdateData
    {
        public byte[] ReceiversIds { get; set; }
        public Message[] MessagesToSend { get; set; }
    }
}