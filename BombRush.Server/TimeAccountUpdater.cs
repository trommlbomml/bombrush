using System;
using System.Diagnostics;
using System.Threading;

namespace BombRush.Server
{
    class TimeAccountUpdater
    {
        private readonly Stopwatch _stopwatch;
        private readonly TimeSpan _pauseTime;
        private readonly TimeSpan _frameTimeSeconds;
        private TimeSpan _timeAccount;
        
        public TimeAccountUpdater(int frameRate, int pauseTimeMilliseconds)
        {
            _pauseTime = TimeSpan.FromMilliseconds(pauseTimeMilliseconds);
            _frameTimeSeconds = TimeSpan.FromSeconds(1.0d / frameRate);
            _timeAccount = TimeSpan.Zero;
            _stopwatch = Stopwatch.StartNew();   
        }

        public void Update(Action<float> onUpdate)
        {
            var currentTime = _stopwatch.Elapsed;

            while (_timeAccount >= _frameTimeSeconds)
            {
                onUpdate((float) _frameTimeSeconds.TotalSeconds);
                _timeAccount -= _frameTimeSeconds;
            }

            var dt = _stopwatch.Elapsed - currentTime;
            _timeAccount += dt;

            if (_timeAccount <= _frameTimeSeconds - _pauseTime)
            {
                currentTime = _stopwatch.Elapsed;
                Thread.Sleep(_pauseTime);
                dt = _stopwatch.Elapsed - currentTime;
                _timeAccount += dt;
            }
        }
    }
}
