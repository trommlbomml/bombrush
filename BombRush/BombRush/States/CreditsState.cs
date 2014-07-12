using BombRush.Gui;
using Game2DFramework.Extensions;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BombRush.States
{
    class CreditsState : BackgroundState
    {
        private static readonly string[] Data = 
        {
            "Programming", 
            "Peter Friedland",
            "",
            "Special Thanks to",
            "Andre Mueller",
            "",
            "Press ESC to return to main menu"
        };
        
        private TitledBorder _titledBorder;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);
            _titledBorder = new TitledBorder(Game, "Credits");
        }

        public override void OnLeave()
        {
        }

        protected override void OnEntered(object enterInformation)
        {
            _titledBorder.SetClientSize(400, Data.Length * Resources.NormalFont.LineSpacing, BombGame.MenuStartY);
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            if (Game.Keyboard.IsKeyDownOnce(Keys.Escape))
                return StateChangeInformation.StateChange(typeof (MainMenuState), BlendTransition.Id);

            return StateChangeInformation.Empty;
        }

        private void DrawCentered(SpriteBatch spriteBatch, string text, float y)
        {
            var width = Resources.NormalFont.MeasureString(text).X;
            spriteBatch.DrawString(Resources.NormalFont, text, new Vector2((Game.ScreenWidth - width) * 0.5f, y).SnapToPixels(), Color.White);
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _titledBorder.Draw(Game.SpriteBatch, true);

            float startY = _titledBorder.ClientRectangle.Y;
            foreach (var line in Data)
            {
                if (!string.IsNullOrEmpty(line)) DrawCentered(Game.SpriteBatch, line, startY);
                startY += Resources.NormalFont.LineSpacing;
            }
        }
    }
}
