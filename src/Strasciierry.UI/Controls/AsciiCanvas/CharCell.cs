using CommunityToolkit.Mvvm.ComponentModel;

namespace Strasciierry.UI.Controls.AsciiCanvas;

public partial class CharCell : ObservableObject
{
    public int Column { get; }
    public int Row { get; }

    [ObservableProperty]
    public partial char Character { get; set; } = ' ';

    public CharCell(int column, int row)
    {
        Column = column;
        Row = row;
    }
}
