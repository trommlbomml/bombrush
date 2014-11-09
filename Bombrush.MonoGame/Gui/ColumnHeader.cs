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
}