namespace BombRush.Logic.AI
{
    interface ITask
    {
        void Start();
        bool Finished { get; }
        void Update(float elapsed, ComFigureController controller);
    }
}
