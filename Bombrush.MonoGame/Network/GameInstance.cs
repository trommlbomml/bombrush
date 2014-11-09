namespace Bombrush.MonoGame.Network
{
    class GameInstance
    {
        public byte SessionId { get; set; }
        public string Name { get; set; }
        public int PlayerCount { get; set; }
        public bool IsRunning { get; set; }
    }
}