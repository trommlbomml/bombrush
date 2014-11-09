
using Game2DFramework.Interaction;

namespace BombRush.Logic.AI.Tasking
{
    class WaitTask : ITask
    {
        private readonly ActionTimer _timer;

        public WaitTask(float time)
        {
            _timer = new ActionTimer(time, true);
        }

        public void Start()
        {
            _timer.Start(d => Finished = true);
        }

        public bool Finished { get; private set; }

        public void Update(float elapsed, ComFigureController controller)
        {
            if (Finished) return;

            _timer.Update(0, elapsed);
        }
    }
}
