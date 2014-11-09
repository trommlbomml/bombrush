
using System;

namespace Bombrush.MonoGame.Gui2
{
    struct Margin
    {
        public static readonly Margin Empty = new Margin();

        public int Top;
        public int Bottom;
        public int Left;
        public int Right;

        public int Vertical { get { return Top + Bottom; } }
        public int Horizontal { get { return Left + Right; } }

        public Margin(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Margin(int vertical, int horizontal)
        {
            Left = horizontal;
            Right = horizontal;
            Top = vertical;
            Bottom = vertical;
        }

        public Margin(int distance)
        {
            Left = distance;
            Right = distance;
            Top = distance;
            Bottom = distance;
        }

        public static Margin Parse(string text)
        {
            var token = text.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            if (token.Length == 1) return new Margin(int.Parse(token[0]));
            if (token.Length == 2) return new Margin(int.Parse(token[0]), int.Parse(token[1]));
            if (token.Length == 4) return new Margin(int.Parse(token[0]), int.Parse(token[1]), int.Parse(token[2]), int.Parse(token[3]));

            throw new ArgumentException("string is not parsable margin", "text");
        }
    }
}
