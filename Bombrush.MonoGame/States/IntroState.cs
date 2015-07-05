using Game2DFramework.Drawing;
using Game2DFramework.Interaction;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.States
{
    internal class IntroState : InitializableState
    {
        private Animator _iconAnimator;
        private Sprite _iconSprite;
        private StateChangeInformation _stateChangeInformation;

        protected override void OnInitialize(object enterInformation)
        {
            _iconSprite = new Sprite(Game.Content.Load<Texture2D>("Textures/MonoGameLogo"))
            {
                Position = new Vector2(Game.ScreenWidth*0.5f, Game.ScreenHeight*0.5f),
                Alpha = 0.0f
            };

            _iconAnimator = new Animator();
            _iconAnimator.AddAnimation("BlendIn", new Animation(1.0f, AnimateBlendIn));
            _iconAnimator.AnimationFinished += () => Game.AddDelayedAction(OnStartTransition, 1.0f);
        }

        protected override void OnEntered(object enterInformation)
        {
            Game.Cursor.IsActive = false;
            _iconAnimator.PlayAnimation("BlendIn");
            _stateChangeInformation = StateChangeInformation.Empty;
        }

        private void OnStartTransition()
        {
            _stateChangeInformation = StateChangeInformation.StateChange(typeof (MainMenuState),
                typeof (BlendTransition));
        }

        private void AnimateBlendIn(float delta)
        {
            _iconSprite.Rotation = MathHelper.SmoothStep(0.0f, 1.0f, delta)*MathHelper.TwoPi;
            _iconSprite.SetScale(MathHelper.SmoothStep(0.0f, 1.0f, delta));
            _iconSprite.Alpha = delta;
        }

        public override void OnLeave()
        {
            Game.Cursor.IsActive = true;
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            _iconAnimator.Update(elapsedTime);

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            Game.GraphicsDevice.Clear(Color.Black);
            _iconSprite.Draw(Game.SpriteBatch);
        }
    }
}
