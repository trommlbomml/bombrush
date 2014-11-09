using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game2DFramework;
using Microsoft.Xna.Framework;

namespace Bombrush.MonoGame.Gui
{
    class DataTableView : GameObject
    {
        private readonly int _rowHeight;
        private int _currentRowCount;
        private readonly Func<int> _getRowCount;
        private readonly Action<int, DataTableRow> _updateTableRow;
        private readonly List<DataTableRow> _rows;
        private readonly List<Tuple<string, int>> _columnsWithNameAndWidth;
        private int _selectedRowIndex;

        public int MaxVisibleRowCount { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int CellPadding { get; set; }

        public int Height
        {
            get
            {
                return (MaxVisibleRowCount+1)*_rowHeight;
            }
        }

        public int Width
        {
            get { return _columnsWithNameAndWidth.Sum(c => c.Item2); }
        }

        public DataTableView(Game2D game, Func<int> getRowCount, Action<int, DataTableRow> updateTableRow)
            : base(game)
        {
            _rows = new List<DataTableRow>();

            CellPadding = 5;
            MaxVisibleRowCount = 10;
            SelectedRowIndex = -1;

            _rowHeight = Resources.NormalFont.LineSpacing + 2 * CellPadding;
            _getRowCount = getRowCount;
            _updateTableRow = updateTableRow;
            _columnsWithNameAndWidth = new List<Tuple<string, int>>();
        }

        public int SelectedRowIndex
        {
            get { return _selectedRowIndex; }
            set
            {
                if (value < -1)throw new InvalidDataException("SelectedRowIndex cannot be smaller than -1");
                _selectedRowIndex = value;
            }
        }

        public void AddColumn(string headerName, int width)
        {
            _columnsWithNameAndWidth.Add(new Tuple<string, int>(headerName, width));
        }

        public void Update()
        {
            _currentRowCount = _getRowCount();
            ResizeRowListToElementCount();

            UpdateRows();
        }

        private void UpdateRows()
        {
            for (var i = 0; i < _currentRowCount; i++)
            {
                _updateTableRow(i, _rows[i]);
            }
        }

        private void ResizeRowListToElementCount()
        {
            if (_currentRowCount > _rows.Count)
            {
                _rows.AddRange(Enumerable.Range(1, _currentRowCount - _rows.Count)
                     .Select(i => new DataTableRow(_columnsWithNameAndWidth.Count)));
            }
        }

        public void Draw()
        {
            var currentY = Y;

            for (var y = -1; y < MaxVisibleRowCount; y++)
            {
                var currentX = X;

                if (SelectedRowIndex != -1 && SelectedRowIndex == y)
                {
                    Game.ShapeRenderer.DrawFilledRectangle(currentX, currentY, Width, _rowHeight, Color.CornflowerBlue);
                }

                for (var x = 0; x < _columnsWithNameAndWidth.Count; x++)
                {
                    var currentColumnWidth = _columnsWithNameAndWidth[x].Item2;
                    Game.ShapeRenderer.DrawRectangle(currentX,currentY,currentColumnWidth, _rowHeight, Color.White);

                    if (y < _currentRowCount)
                    {
                        var text = y == -1 ? _columnsWithNameAndWidth[x].Item1 : _rows[y].Columns[x];

                        if (!string.IsNullOrEmpty(text))
                        {
                            Game.SpriteBatch.DrawString(Resources.NormalFont, text, new Vector2(currentX + CellPadding, currentY + CellPadding), Color.White);
                        }   
                    }

                    currentX += currentColumnWidth;
                }

                currentY += _rowHeight;
            }
        }
    }
}
