
using System;
using BombRush.Network.Framework;
using Game2DFramework;

namespace BombRush.Gui
{
    class ToggleActionMenuItem : ActionMenuItem
    {
        public string TrueText { get; set; }
        public string FalseText { get; set; }

        private bool _isTrue;
        public bool IsTrue
        {
            get { return _isTrue; }
            set
            {
                _isTrue = value;
                Text = IsTrue ? TrueText : FalseText;
            }
        }

        public ToggleActionMenuItem(Game2D game, string trueText, string falseText, bool isTrue, Action onAction) : 
            base(game, string.Empty, onAction, ActionTriggerKind.None)
        {
            TrueText = trueText;
            FalseText = falseText;
            IsTrue = isTrue;
        }

        public override void FireAction()
        {
            IsTrue = !IsTrue;
            Text = IsTrue ? TrueText : FalseText;
            base.FireAction();
        }

        public override float GetMaxWidth()
        {
            return Math.Max(
                string.IsNullOrEmpty(TrueText) ? 0 : Resources.BigFont.MeasureString(TrueText).X,
                string.IsNullOrEmpty(FalseText) ? 0 : Resources.BigFont.MeasureString(FalseText).X);
        }
    }
}
