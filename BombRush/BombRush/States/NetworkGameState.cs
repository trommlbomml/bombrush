using System;
using Game2DFramework.States;

namespace BombRush.States
{
    class NetworkGameState : BackgroundState
    {
        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);
        }

        protected override void OnEntered(object enterInformation)
        {
            throw new NotImplementedException();
        }

        public override void OnLeave()
        {
            throw new NotImplementedException();
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            return base.OnUpdate(elapsedTime);
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);
        }
    }
}
