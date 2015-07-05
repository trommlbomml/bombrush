using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Gui
{
    abstract class MenuItem : GameObject
    {
        private static readonly Color DisabledColor = new Color(150,150,150);

        protected MenuItem(Game2D game) : base(game)
        {
            IsEnabled = true;
        }

        public bool IsEnabled { get; set; }
        public string Text { get; set; }
        public Vector2 Position { get; set; }
        public SoundEffect AcceptSoundAffect { get; set; }
        public SoundEffect CancelSoundAffect { get; set; }
        public Color FontColor { get { return IsEnabled ? Color.White : DisabledColor; } }

        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract float GetMaxWidth();
    }
}
