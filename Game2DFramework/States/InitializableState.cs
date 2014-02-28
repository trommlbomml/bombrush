
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework.States
{
    public abstract class InitializableState : IState
    {
        private bool _initialized;

        public Game2D Game { get; set; }
        public void OnEnter(object enterInformation)
        {
            if (!_initialized)
            {
                OnInitialize(enterInformation);
                _initialized = true;
            }
            OnEntered(enterInformation);
        }

        protected abstract void OnEntered(object enterInformation);
        protected abstract void OnInitialize(object enterInformation);

        public abstract void OnLeave();
        public abstract StateChangeInformation OnUpdate(float elapsedTime);
        public abstract void OnDraw(float elapsedTime);

        public RenderTarget2D TransitionRenderTarget { get; set; }
    }
}
