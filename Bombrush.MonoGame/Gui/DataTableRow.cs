namespace BombRush.Gui
{
    class DataTableRow
    {
        public string[] Columns;

        public DataTableRow(int columnCount)
        {
            Columns = new string[columnCount];
        }
    }
}