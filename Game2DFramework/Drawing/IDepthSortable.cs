
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework.Drawing
{
    public interface IDepthSortable
    {
        int Depth { get; }
        void Draw(SpriteBatch spriteBatch);
    }
}
