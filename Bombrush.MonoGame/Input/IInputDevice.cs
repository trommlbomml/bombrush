
namespace Bombrush.MonoGame.Input
{
    interface IInputDevice
    {
        bool IsDown(InputKey inputKey);
        bool IsDownOnce(InputKey inputKey);
    }
}
