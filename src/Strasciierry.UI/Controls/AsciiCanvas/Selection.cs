using Microsoft.UI.Xaml.Shapes;

namespace Strasciierry.UI.Controls.AsciiCanvas;

class Selection
{
    public Rectangle Area { get; set; }
    public int InitialRow { get; set; }
    public int InitialColumn { get; set; }
    public int StartRow { get; set; }
    public int StartColumn { get; set; }
}