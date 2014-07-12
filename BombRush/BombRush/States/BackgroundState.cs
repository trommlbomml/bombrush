using BombRush.Gui;
using BombRush.Network.Framework;
using BombRush.Rendering;
using Game2DFramework.Extensions;
using Game2DFramework.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.States
{
    abstract class BackgroundState : InitializableState
    {
        private Texture2D _headingTexture;
        private SplashBackground _splashBackground;

        protected Cursor Cursor { get; private set; }

        protected override void OnInitialize(object enterInformation)
        {
            Cursor = new Cursor(Game);
            _splashBackground = new SplashBackground(Game, Game.Content.Load<Texture2D>("textures/splash")) { ModulateColor = new Color(0.0f, 0.8f, 0.0f) };
            _headingTexture = Game.Content.Load<Texture2D>("textures/headings");
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            Cursor.Update();
            _splashBackground.Update(elapsedTime);
            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            _splashBackground.Draw(Game.SpriteBatch);

            var position = new Vector2(Game.ScreenWidth*0.5f - 150.0f, 10.0f);
            Game.SpriteBatch.Draw(_headingTexture, position, new Rectangle(0,0,300,45), Color.White);
            
            const string version = "0.9 beta";
            var size = Resources.NormalFont.MeasureString(version);

            Game.SpriteBatch.DrawString(Resources.NormalFont, 
                                   version,
                                   new Vector2(Game.ScreenWidth - size.X - 5, Game.ScreenHeight - size.Y - 5).SnapToPixels(),
                                   Color.White);
        }
    }
}
