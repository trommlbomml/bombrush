
using Game2DFramework;

namespace Bombrush.MonoGame.Gui
{
    class TimeMenuItem : NumericMenuItem
    {
        public TimeMenuItem(Game2D game, string text, int min, int max) : base(game, text, min, max, false)
        {
            Step = 10;
        }

        protected override string FullText
        {
            get
            {
                if (CurrentValue == 0)
                    return string.Format("{0} <Infinite>", Text);
                return string.Format("{0} <{1:00}:{2:00}>", Text, CurrentValue / 60, CurrentValue % 60);
            }
        }
    }
}
