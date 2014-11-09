using System.Linq;
using BombRush.Network.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush
{
    internal static class Resources
    {
        private static readonly int[] ResolutionHeights = new[] { 480, 768 };

        private static ResolutionResource<SpriteFont> _normalFonts;
        private static ResolutionResource<SpriteFont> _bigFonts;
        private static ResolutionResource<SpriteFont> _playerNameFonts;

        public static SpriteFont NormalFont { get; private set; }
        public static SpriteFont BigFont { get; private set; }
        public static SpriteFont PlayerNameFont { get; private set; }

        public static void LoadContent(ContentManager content)
        {
            _normalFonts = new ResolutionResource<SpriteFont>(content, "fonts/NormalFont", ResolutionHeights);
            _bigFonts = new ResolutionResource<SpriteFont>(content, "fonts/BigFont", ResolutionHeights);
            _playerNameFonts = new ResolutionResource<SpriteFont>(content, "fonts/PlayerNameFont", ResolutionHeights);
        }

        public static void ChangeResolution(int width, int height)
        {
            var key = EvaluateHeightKey(height);
            NormalFont =_normalFonts[key];
            BigFont = _bigFonts[key];
            PlayerNameFont = _playerNameFonts[key];
        }

        private static int EvaluateHeightKey(int height)
        {
            for (var i = 0; i < ResolutionHeights.Length; i++)
            {
                if (ResolutionHeights[i] == height)
                    return height;
                if (ResolutionHeights[i] > height)
                    return ResolutionHeights[i - 1];
            }
            return ResolutionHeights.Last();
        }
    }
}
