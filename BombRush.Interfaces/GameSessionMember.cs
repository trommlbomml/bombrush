
namespace BombRush.Interfaces
{
    public enum MemberType
    {
        ActivePlayer,
        Computer,
        Watcher,
    }

    public interface GameSessionMember
    {
        byte Id { get; }
        string Name { get; }
        MemberType Type { get; }
        int PlayerIndex { get; }
        int Wins { get; }
    }
}
