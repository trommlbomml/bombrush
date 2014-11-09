using System.Collections.Generic;
using System.Linq;
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bombrush.MonoGame.Gui
{
    class StackedMenu : GameObject
    {
        private const float Spacing = 10;

        private readonly AnimatedBomb _animatedBomb;
        private readonly List<MenuItem> _menuItems;
        private int _currentMenuIndex;
        private readonly TitledBorder _titledBorder;

        public Keys CancelKey { get; set; }
        public string Title { get { return _titledBorder.Title; } set { _titledBorder.Title = value; } }
        public int FirstMenuItemStartOffset { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        private float SpacePerItem { get { return Resources.BigFont.LineSpacing + Spacing; } }
        
        public StackedMenu(Game2D game) : base(game)
        {
            _menuItems = new List<MenuItem>();
            _animatedBomb = new AnimatedBomb(game.Content);
            _titledBorder = new TitledBorder(game);
            Height = int.MinValue;
            Width = int.MinValue;
            FirstMenuItemStartOffset = 0;
            CancelKey = Keys.Escape;
        }

        public T AppendMenuItem<T>(T item) where T : MenuItem
        {
            _menuItems.Add(item);
            UpdateMenuItemPositions();
            UpdateBombPosition();

            return item;
        }

        public void InsertMenuItemAt(int index, MenuItem item)
        {
            _menuItems.Insert(index, item);
            UpdateBombPosition();
            UpdateMenuItemPositions();
        }

        public void RemoveMenuItem(MenuItem item)
        {
            if(_menuItems.Remove(item))
            {
                UpdateBombPosition();
                UpdateMenuItemPositions();
            }
        }

        public void Update(float elapsed)
        {
            _animatedBomb.Update(elapsed);

            if (Game.Keyboard.IsKeyDownOnce(Keys.Down))
            {
                NextMenuItem(true);
                UpdateBombPosition();
            }
            else if (Game.Keyboard.IsKeyDownOnce(Keys.Up))
            {
                PreviousMenuItem(true);
                UpdateBombPosition();
            }

            if (Game.Keyboard.IsKeyDownOnce(CancelKey))
            {
                ActionMenuItem menuItem = _menuItems.OfType<ActionMenuItem>().FirstOrDefault(a => a.ActionKind == ActionTriggerKind.IsCancel);
                if (menuItem != null)
                    menuItem.FireAction();
            }

            if (_menuItems.Count > 0)
                _menuItems[_currentMenuIndex].Update(elapsed);
        }

        private void NextMenuItem(bool checkEnabled)
        {
            if (++_currentMenuIndex == _menuItems.Count)
                _currentMenuIndex = 0;

            if (checkEnabled)
            {
                if (!_menuItems[_currentMenuIndex].IsEnabled)
                    NextMenuItem(false);
            }
        }

        private void PreviousMenuItem(bool checkEnabled)
        {
            if (--_currentMenuIndex < 0)
                _currentMenuIndex = _menuItems.Count - 1;

            if (checkEnabled)
            {
                if (!_menuItems[_currentMenuIndex].IsEnabled)
                    PreviousMenuItem(false);
            }
        }

        public T GetMenuItem<T>(int index) where T : MenuItem
        {
            return (T) _menuItems[index];
        }

        public void SelectItem(int index)
        {
            if (index >= 0 && index < _menuItems.Count)
            {
                _currentMenuIndex = index;
                UpdateBombPosition();
            }
        }

        public void SelectFirstMenuItem()
        {
            SelectItem(0);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _titledBorder.Draw(spriteBatch, true);
            foreach (MenuItem menuItem in _menuItems)
            {
                menuItem.Draw(spriteBatch);
            }
            if (_currentMenuIndex >= 0 && _currentMenuIndex < _menuItems.Count && _menuItems[_currentMenuIndex].IsEnabled)
                _animatedBomb.Draw(spriteBatch);
        }

        public Rectangle ClientRectangle { get { return _titledBorder.ClientRectangle; } }

        private const int MiddledHorizontalBombDistance = 40;

        public void RecalculatePositions()
        {
            UpdateMenuItemPositions();
            UpdateBombPosition();
        }

        private void UpdateBombPosition()
        {
            if (_currentMenuIndex == _menuItems.Count)
                _currentMenuIndex--;

            float maxWidth = _menuItems.Max(m => m.GetMaxWidth());
            _animatedBomb.Position = new Vector2(
                (Game.ScreenWidth - maxWidth) / 2 - MiddledHorizontalBombDistance / 2,
                _titledBorder.ClientRectangle.Y + FirstMenuItemStartOffset + (_currentMenuIndex * SpacePerItem) + (int)(Resources.BigFont.LineSpacing * 0.5f));
        }

        private void UpdateMenuItemPositions()
        {
            int height = Height == int.MinValue ? (int)(SpacePerItem * _menuItems.Count - Spacing) + FirstMenuItemStartOffset : Height;
            int width = (int)MathHelper.Max(Width, (int)_menuItems.Max(m => m.GetMaxWidth()) + MiddledHorizontalBombDistance*2);

            _titledBorder.SetClientSize(width, height, BombGame.MenuStartY);

            float startX = Game.ScreenWidth * 0.5f;
            float startY = _titledBorder.ClientRectangle.Y + FirstMenuItemStartOffset;
            foreach (var menuItem in _menuItems)
            {
                menuItem.Position = new Vector2(startX, startY);
                startY += SpacePerItem;
            }
        }
    }
}
