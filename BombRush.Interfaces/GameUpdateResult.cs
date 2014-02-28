namespace BombRush.Interfaces
{
    public enum GameUpdateResult
    {
        None = 0,
        SwitchToPrepareMatch = 1,
        SwitchToGame = 2,
        MatchFinished = 3,
        NextMatch = 4,
        GameOver = 5,
        ServerShutdown = 6,
        NoUniqueName = 7,
        ConnectedToServer = 8,
    }
}