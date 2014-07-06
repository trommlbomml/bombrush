namespace BombRush.Server
{
    interface LogListener
    {
        void PrintInfo(string text);
        void PrintWarning(string text);
    }
}
