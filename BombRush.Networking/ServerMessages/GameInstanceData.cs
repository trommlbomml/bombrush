namespace BombRush.Networking.ServerMessages
{
    public struct GameInstanceData
    {
        public byte SessionId { get; set; }
        public string Name { get; set; }
        public byte PlayerCount { get; set; }
        public bool IsRunning { get; set; }
    }
}