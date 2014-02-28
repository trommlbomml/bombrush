
namespace BombRush.Logic.AI.Tasking
{
    class PlaceBombTask : ITask
    {
        public void Start()
        {
            Finished = false;
        }

        public bool Finished { get; private set; }

        public void Update(float elapsed, ComFigureController controller)
        {
            if (Finished)
                return;

            controller.ActionDone = true;
            Finished = true;
        }
    }
}
