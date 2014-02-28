using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace BombRush.Network.Framework
{
    class ResolutionResource<T>
    {
        private readonly Dictionary<int, T> _resourcesByHeightResolution;

        public ResolutionResource(ContentManager content, string baseResourceName, int[] resolutions)
        {
            _resourcesByHeightResolution = new Dictionary<int, T>();
            foreach (var resolution in resolutions)
            {
                _resourcesByHeightResolution.Add(resolution, content.Load<T>(string.Format("{0}{1}", baseResourceName, resolution)));
            }
        }

        public T this[int resolution]
        {
            get
            {
                if (!_resourcesByHeightResolution.ContainsKey(resolution))
                    throw new InvalidOperationException("There is no resource in this resolution");
                return _resourcesByHeightResolution[resolution];
            }
        }
    }
}
