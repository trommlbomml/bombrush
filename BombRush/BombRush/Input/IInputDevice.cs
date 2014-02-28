
namespace BombRush.Input
{
    public enum InputKey
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Action,
        Back,
    }

    interface IInputDevice
    {
        bool IsDown(InputKey inputKey);
        bool IsDownOnce(InputKey inputKey);
    }
}
