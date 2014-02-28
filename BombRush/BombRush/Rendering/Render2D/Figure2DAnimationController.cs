
using BombRush.Interfaces;
using Microsoft.Xna.Framework;

namespace BombRush.Rendering.Render2D
{
    class Figure2DAnimationController
    {
        public const float DieAnimationDuration = 1.0f;
        public const float IdleAnimationTime = 0.5f;
        public const float OverallDieAnimationDuration = DieAnimationDuration + IdleAnimationTime;

        private bool _wasAlive = true;
        private float _yDistanceForDie;
        private Vector2 _startDiePosition;

        public int StepFrame;
        public float Elapsed;
        public Vector2 DieAnimationPosition;
        public float DieAnimationTime { get; private set; }
        public int YDistanceForDie { get { return (int) _yDistanceForDie; } }
        
        public void UpdateAnimation(Figure figure, float elapsed, Vector2 centeredPosition, Vector2 offset)
        {
            const float animationFrameTime = 0.2f;

            if (figure.IsAlive)
            {
                if (figure.WalkDirection != Vector2.Zero)
                {
                    Elapsed += elapsed;
                    if (Elapsed > animationFrameTime)
                    {
                        Elapsed -= animationFrameTime;
                        StepFrame = StepFrame == 1 ? 2 : 2 - StepFrame;
                    }
                }
                else
                {
                    StepFrame = 1;
                    Elapsed = 0;
                }    
            }
            else
            {
                if (_wasAlive)
                {
                    DieAnimationTime = 0;
                    _startDiePosition = new Vector2(centeredPosition.X + offset.X, -16.0f);
                    DieAnimationPosition = _startDiePosition;
                    _yDistanceForDie = centeredPosition.Y + offset.Y - _startDiePosition.Y;
                }
                else
                {
                    DieAnimationTime += elapsed;
                    if (DieAnimationTime < IdleAnimationTime)
                        DieAnimationPosition = _startDiePosition;
                    else
                    {
                        DieAnimationPosition.Y = _startDiePosition.Y + MathHelper.SmoothStep(0, _yDistanceForDie, (DieAnimationTime - IdleAnimationTime) / DieAnimationDuration);
                    }
                }
            }

            _wasAlive = figure.IsAlive;
        }
    }
}
