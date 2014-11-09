namespace Bombrush.MonoGame.Gui
{
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
}