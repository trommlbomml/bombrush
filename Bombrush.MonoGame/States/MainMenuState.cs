using System;
using Bombrush.MonoGame.Gui;
using Bombrush.MonoGame.Gui2;
using Game2DFramework.Drawing;
using Game2DFramework.Gui2;
using Game2DFramework.Gui2.Controls;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.States
{
    class MainMenuState : BackgroundState
    {
        private StateChangeInformation _stateChangeInformation;
        private TimedSplash _splash;

        private UiOverlay _mainOverlay;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);
            _splash = new TimedSplash(Game);

            _mainOverlay = new UiOverlay();
            _mainOverlay.AddElement(new NinePatchImage(new NinePatchSprite(Game.Content.Load<Texture2D>("textures/border"), 12, 12), new Rectangle(0, 200, 200, 220)));
            _mainOverlay.AddElement(new Button2(Game, "Local Game", new Rectangle(20, 215, 160, 35), () => DoTransition(typeof(LocalGameConfigurationState))));
            _mainOverlay.AddElement(new Button2(Game, "Network Game", new Rectangle(20, 250, 160, 35), () => DoTransition(typeof(NetworkGameState))));
            _mainOverlay.AddElement(new Button2(Game, "Options", new Rectangle(20, 285, 160, 35), () => DoTransition(typeof(OptionMenuState))));
            _mainOverlay.AddElement(new Button2(Game, "Credits", new Rectangle(20, 320, 160, 35), () => DoTransition(typeof(CreditsState))));
            _mainOverlay.AddElement(new Button2(Game, "Quit", new Rectangle(20, 355, 160, 35), OnQuitClicked));
            _mainOverlay.Center(Game.GetScreenRectangle(),  false, true);
            _mainOverlay.AddInputController(new KeyboardInputController(Game));
        }

        private void DoTransition(Type targetState)
        {
            _stateChangeInformation = StateChangeInformation.StateChange(targetState, SlideTransition.Id);
        }

        private void OnQuitClicked()
        {
            _stateChangeInformation = StateChangeInformation.QuitGameInformation(ZappoutTransition.Id);
        }

        protected override void OnEntered(object enterInformation)
        {
            _stateChangeInformation = StateChangeInformation.Empty;
            _mainOverlay.Show();

            if (enterInformation is bool && (bool)enterInformation)
            {
                _splash.Start("Server Connection Lost", 1.5f);
            }
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            _stateChangeInformation = StateChangeInformation.Empty;

            _splash.Update(elapsedTime);
            if (!_splash.Running) _mainOverlay.Update(new GameTime(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(elapsedTime)));

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);
            _mainOverlay.Draw(Game.SpriteBatch);
            _splash.Draw(Game.SpriteBatch);
        }
    }
}
