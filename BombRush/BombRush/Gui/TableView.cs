
using System.Collections.Generic;
using BombRush.Network.Framework;
using BombRush.Rendering;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace BombRush.Gui
{
    struct ColumnHeader
    {
        public string Caption;
        public int Width;

        public ColumnHeader(string caption, int width)
        {
            Caption = caption;
            Width = width;
        }
    }

    struct Cell
    {
        public string Text;
        public byte HeadId;

        public Cell(string text, byte headId = (byte)0)
        {
            Text = text;
            HeadId = headId;
        }
    }

    struct Row
    {
        public Cell[] Cells;
        public Color? Color;
    }

    class TableView : GameObject
    {
        public const int CellPadding = 5;

        private readonly List<ColumnHeader> _columnHeaders;
        private readonly List<Row> _rows;

        public Vector2 Start { get; set; }
        public Color HeaderColor { get; set; }
        public Color ContentColor { get; set; }        
        public int Rows { get; set; }

        public void Clear()
        {
            _rows.Clear();            
        }

        public TableView(Game2D game) : base(game)
        {
            _columnHeaders = new List<ColumnHeader>();
            HeaderColor = Color.White;
            ContentColor = Color.White;
            _rows = new List<Row>();
        }

        public void AddColumn(string header, int width)
        {
            _columnHeaders.Add(new ColumnHeader(header, width));
            if (_rows.Count > Rows)
                Rows = _rows.Count;
        }

        public void AddRow(Cell[] data, Color ?color = null)
        {
            _rows.Add(new Row {Color = color, Cells = data});
        }

        public int RowHeight
        {
            get { return Resources.NormalFont.LineSpacing + 2*CellPadding; }
        }

        public int Height 
        {
            get { return (_rows.Count + 1)*RowHeight; }
        }

        public int Width
        {
            get { return _columnHeaders.Sum(c => c.Width); }
        }

        public List<Row> Data
        {
            get { return _rows; }
        }

        private void DrawGrid()
        {
            var currentX = (int)Start.X;
            foreach (var columnHeader in _columnHeaders)
            {
                for (var i = 0; i <= Rows; i++)
                {
                    Game.ShapeRenderer.DrawRectangle(currentX, (int)Start.Y + i * RowHeight, columnHeader.Width, RowHeight, Color.White);
                }
                currentX += columnHeader.Width;
            }
        }

        private void DrawContent(SpriteBatch spriteBatch)
        {
            int currentX = 0;
            for(int columnIndex = 0; columnIndex < _columnHeaders.Count; columnIndex++)
            {
                ColumnHeader column = _columnHeaders[columnIndex];
                DrawCellContent(spriteBatch, column.Caption, new Vector2(currentX, 0), HeaderColor);

                for (int i = 0; i < _rows.Count; i++)
                {
                    Cell cell = _rows[i].Cells[columnIndex];
                    int indent = cell.HeadId > 0 ? RowHeight + 2 : 0;
                    Vector2 position = new Vector2(currentX, (i + 1)*RowHeight);

                    DrawCellContent(spriteBatch,
                                    cell.Text,
                                    position + new Vector2(indent, 0),
                                    _rows[i].Color.GetValueOrDefault(ContentColor));

                    if (cell.HeadId <= 0) continue;
                    var p = (Start + position + new Vector2(1)).ToPoint();
                    Heads.Draw(spriteBatch, cell.HeadId, p.X, p.Y, RowHeight - 2, RowHeight - 2);
                }
                currentX += column.Width;
            }
        }

        private void DrawCellContent(SpriteBatch spriteBatch, string text, Vector2 localstart, Color color)
        {
            if (string.IsNullOrEmpty(text))
                return;

            spriteBatch.DrawString(Resources.NormalFont, text, Start + localstart + new Vector2(CellPadding), color);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawGrid();
            DrawContent(spriteBatch);
        }

    }

}
