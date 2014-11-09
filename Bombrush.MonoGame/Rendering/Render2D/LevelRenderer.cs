using BombRush.Interfaces;
using Game2DFramework;
using Microsoft.Xna.Framework;

namespace Bombrush.MonoGame.Rendering.Render2D
{
    class LevelRenderer : GameObject
    {
        private readonly Tileset _tileset;
        private readonly Level _gameLevelLogic;
        private readonly Vector2 _centeringOffset;

        public LevelRenderer(Game2D game, Level gameLevelLogic, Vector2 centeringOffset) : base(game)
        {
            _tileset = new Tileset(game.Content, "tilesets/tileset1");
            _gameLevelLogic = gameLevelLogic;
            _centeringOffset = centeringOffset;
        }

        public void DrawBaseLayer()
        {
            foreach (var tileBlock in _gameLevelLogic.Data)
            {
                DrawTileBlock(tileBlock);
            }
        }

        public void DrawFringeLayer()
        {
            foreach (var tileBlock in _gameLevelLogic.Fringe)
            {
                DrawTileBlock(tileBlock);
            }
        }

        private void DrawTileBlock(TileBlock tileBlock)
        {
            if (!tileBlock.IsActive) return;
            //todo: animated tile blocks
            Rectangle currentRectangle = _tileset.Templates[tileBlock.Type].Rectangles[0]; // (IsAnimated ? Rectangles[_currentFrame] : Rectangles[0]);
            Vector2 position = _gameLevelLogic.GetWorldFromTilePosition(tileBlock.TilePosition);

            Game.SpriteBatch.Draw(_tileset.TileSetTexture, position + _centeringOffset, currentRectangle, Color.White);
        }
    }
}
