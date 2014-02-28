using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework.Drawing
{
    public class DepthRenderer
    {
        private readonly List<IDepthSortable> _depthSortables = new List<IDepthSortable>(); 
        
        public void Clear()
        {
            _depthSortables.Clear();
        }
        
        public void Register(IDepthSortable depthSortable)
        {
            _depthSortables.Add(depthSortable);
        }

        public void UnRegister(IDepthSortable depthSortable)
        {
            _depthSortables.Remove(depthSortable);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _depthSortables.Sort(CompareDepth);

            foreach (var depthSortable in _depthSortables)
            {
                depthSortable.Draw(spriteBatch);
            }
        }

        private static int CompareDepth(IDepthSortable d1, IDepthSortable d2)
        {
            if (d1.Depth > d2.Depth)
                return 1;
            if (d1.Depth < d2.Depth)
                return -1;
            return 0;
        }
    }
}
