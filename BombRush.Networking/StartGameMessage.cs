namespace BombRush.Networking
{
    public class StartGameMessage : Message
    {
        public StartGameMessage()
        {
            
        }

        public StartGameMessage(double timeStamp)
            : base(timeStamp)
        {
            
        }
    }
}
