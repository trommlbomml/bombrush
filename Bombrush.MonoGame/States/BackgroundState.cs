using Bombrush.MonoGame.Gui;
using Game2DFramework.Drawing;
using Game2DFramework.Extensions;
using Game2DFramework.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.States
{
    abstract class BackgroundState : InitializableState
    {
        private const string Version = "0.9 beta";

        private Sprite _headingSprite;
        private SpriteText _versionText;
        private SplashBackground _splashBackground;

        protected override void OnInitialize(object enterInformation)
        {
            _splashBackground = new SplashBackground(Game) { ModulateColor = new Color(0.0f, 0.8f, 0.0f) };
            _headingSprite = new Sprite(Game.Content.Load<Texture2D>("textures/headings"), new Rectangle(0, 0, 300, 45))
            {
                Position = new Vector2(Game.ScreenWidth*0.5f - 150.0f, 10.0f)
            };
            _versionText = new SpriteText(Resources.NormalFont, Version);
            _versionText.Position = (Game.ScreenSize - _versionText.TextSize -new Vector2(5)).SnapToPixels();
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            _splashBackground.Update(elapsedTime);
            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            _splashBackground.Draw();
            _headingSprite.Draw(Game.SpriteBatch);
            _versionText.Draw(Game.SpriteBatch);
        }
    }
}
