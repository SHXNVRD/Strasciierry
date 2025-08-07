namespace Strasciierry.UI.Controls.AsciiCanvas;

public struct Selection
{
    public int InitialRow { get; set; }
    public int InitialColumn { get; set; }
    public int StartRow { get; set; }
    public int StartColumn { get; set; }
    public int Columns { get; set; }
    public int Rows { get; set; }
}