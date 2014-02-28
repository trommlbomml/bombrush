namespace BombRush.Interfaces
{
    public enum GameSessionState
    {
        Disconnected,
        Lobby,
        PreparingMatchLoadData,
        PreparingMatchWaitForAllReady,
        PreparingMatchSynchronizeStart,
        InGame,
        MatchResult,
    }
}