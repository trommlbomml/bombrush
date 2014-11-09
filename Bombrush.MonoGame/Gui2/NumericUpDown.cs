using System;
using System.Globalization;
using System.Xml;
using Game2DFramework;
using Microsoft.Xna.Framework;

namespace Bombrush.MonoGame.Gui2
{
    class NumericUpDown : TextBlock
    {
        private const string DownText = "<";
        private const string UpText = ">";

        public NumericUpDown(Game2D game, XmlElement element) : base(game, element)
        {
            HorizontalAlignment = HorizontalAlignment.Left;
        }

        public NumericUpDown(Game2D game) : base(game)
        {
            HorizontalAlignment = HorizontalAlignment.Left;
        }

        public int Value { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        private string GetLengthMeasureStringFromMinMax()
        {
            var maxCount = MaxValue.ToString(CultureInfo.InvariantCulture).Length;
            var minCount = MinValue.ToString(CultureInfo.InvariantCulture).Length;
            var count = Math.Max(maxCount, minCount);

            return new string('9', count);
        }

        private int GetNumericPartWidth()
        {
            var upSize = (int)Math.Round(Resources.BigFont.MeasureString(UpText).X);
            var downSize = (int)Math.Round(Resources.BigFont.MeasureString(DownText).X);
            var contentSize = (int)Math.Round(Resources.BigFont.MeasureString(GetLengthMeasureStringFromMinMax()).X);

            return upSize + downSize + contentSize;
        }

        public override Rectangle GetMinSize()
        {
            var minSize = base.GetMinSize();
            minSize.Width += GetNumericPartWidth();
            return minSize;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
