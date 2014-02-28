
namespace BombRush.Networking
{
    public class SwitchToReceiveDataMessage : Message
    {
        public SwitchToReceiveDataMessage()
        {            
        }

        public SwitchToReceiveDataMessage(double timeStamp)
            : base(timeStamp)
        {
        }
    }
}
