using BombRush.Interfaces;
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render2D
{
    class Game2DRenderer : GameObject, IGameRenderer
    {
        private GameInformation _gameInformation;
        private FigureRenderer _figureRenderer;
        private BombRenderer _bombRenderer;
        private LevelRenderer _levelRenderer;
        private ItemRenderer _itemRenderer;
        private FireExplosionRenderer _explosionRenderer;
        
        private Level _level;
        public Vector2 CenteringOffset;
        
        public Game2DRenderer(Game2D game) :
            base(game)
        {
        }

        public void Initialize(Game2D game, Level level)
        {
            _level = level;
            _gameInformation = new GameInformation(game, level) { Width = BombGame.Tilesize * 15 };

            UserOffset = new Vector2(float.NaN, _gameInformation.Height);
            UserOffset = new Vector2(float.NaN);

            _figureRenderer = new FigureRenderer(Game, level, CenteringOffset);
            _bombRenderer = new BombRenderer(Game, CenteringOffset);
            _levelRenderer = new LevelRenderer(Game, level, CenteringOffset);
            _itemRenderer = new ItemRenderer(Game, CenteringOffset);
            _explosionRenderer = new FireExplosionRenderer(Game, CenteringOffset);
        }

        private Vector2 _userOffset;
        private Vector2 UserOffset
        {
            get { return _userOffset; } 
            set
            {
                if (_userOffset != value)
                {
                    _userOffset = value;

                    var xOffset = float.IsNaN(UserOffset.X) ? (Game.ScreenWidth - BombGame.GameLevelWidth * BombGame.Tilesize) * 0.5f : UserOffset.X;
                    var yOffset = float.IsNaN(UserOffset.Y) ? (Game.ScreenHeight - BombGame.GameLevelHeight * BombGame.Tilesize) * 0.5f : UserOffset.Y;
                    CenteringOffset = new Vector2(xOffset, yOffset);
                }
            }
        }

        public void Update(float elapsedTime)
        {
            
        }

        public void Render(float elapsedTime, SpriteBatch spriteBatch, RenderTarget2D currentTransitionRenderTarget = null)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);

            _levelRenderer.DrawBaseLayer();
            _itemRenderer.RenderItems(_level, elapsedTime);
            _levelRenderer.DrawFringeLayer();
            _figureRenderer.RenderDead(_level, elapsedTime);           
            _bombRenderer.Render(_level);
            _figureRenderer.RenderAlive(_level, elapsedTime);
            _explosionRenderer.Render(_level, elapsedTime);

            _gameInformation.Draw(spriteBatch);
        }
    }
}
