﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Rendering
{
    static class Heads
    {
        private static Texture2D _heads;

        public static void LoadContent(ContentManager content)
        {
            _heads = content.Load<Texture2D>("textures/heads");
        }

        public static void Draw(SpriteBatch spriteBatch, byte id, int x, int y, int width, int height)
        {
            spriteBatch.Draw(_heads, new Rectangle(x,y,width,height), new Rectangle(id*16,0,16,16), Color.White);
        }
    }
}
