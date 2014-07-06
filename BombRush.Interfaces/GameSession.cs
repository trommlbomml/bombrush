
namespace BombRush.Interfaces
{
    public interface GameSession
    {
        string SessionName { get; }
        float RemainingStartupTime { get; }

        Level CurrentLevel { get; }
        MatchResultType CurrentMatchResultType { get; }
        GameSessionState State { get; }
        GameSessionMember[] Members { get; }
        
        GameUpdateResult Update(float elapsedTime);
        void OnQuit();
        void StartMatch();
    }
}
