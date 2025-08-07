namespace Strasciierry.UI.Controls.AsciiCanvas;

public interface IAsciiCanvas
{
    double CellHeight { get; }
    double CellWidth { get; }
    int Columns { get; set; }
    int Rows { get; set; }
    Selection Selection { get; }

    AsciiCanvasCell GetCell(int column, int row);
    AsciiCanvasCell GetDefaultCell();
    AsciiCanvasCell GetStyledCell();
    
    void ClearSelection();
    void SetSelection(Selection selection);
    void ApplyDrawingPropertiesFromCell(AsciiCanvasCell cell);
}