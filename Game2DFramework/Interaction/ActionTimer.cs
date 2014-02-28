using System;

namespace Game2DFramework.Interaction
{
    public class ActionTimer
    {
        private Action<double> _action;
        private readonly float _triggerTime;
        private readonly bool _triggerOnce;
        private float _elapsedTime;

        public ActionTimer(Action<double> action, float triggerTime, bool triggerOnce)
        {
            _action = action;
            _triggerTime = triggerTime;
            _triggerOnce = triggerOnce;
        }

        public ActionTimer(float triggerTime, bool triggerOnce) : this(null, triggerTime, triggerOnce)
        {
        }

        public bool Running { get; private set; }
        public float TotalElapsedTime { get; private set; }

        public void Start(Action<double> action)
        {
            _action = action;
            Running = true;
            _elapsedTime = 0;
            TotalElapsedTime = 0;
        }

        public void Start()
        {
            Start(_action);
        }

        public void Stop()
        {
            Running = false;
        }

        public void Update(double timeStamp, float elapsed)
        {
            if (!Running) return;
            _elapsedTime += elapsed;
            TotalElapsedTime += elapsed;
            if (_elapsedTime >= _triggerTime)
            {
                _action(timeStamp);
                if (_triggerOnce)
                    Stop();
                else
                    _elapsedTime -= _triggerTime;
            }
        }
    }
}
