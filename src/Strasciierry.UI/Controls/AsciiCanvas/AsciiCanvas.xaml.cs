using System.Collections.ObjectModel;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers;
using Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers.Base;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;

namespace Strasciierry.UI.Controls.AsciiCanvas;

public sealed partial class AsciiCanvas : UserControl
{
    public DrawingTool DrawingTool
    {
        get => (DrawingTool)GetValue(DrawingToolProperty);
        set => SetValue(DrawingToolProperty, value);
    }

    public static readonly DependencyProperty DrawingToolProperty =
        DependencyProperty.Register(
            nameof(DrawingTool),
            typeof(DrawingTool),
            typeof(AsciiCanvas), 
            new PropertyMetadata(null));

    public char DrawingChar
    {
        get => (char)GetValue(DrawingCharProperty);
        set
        {
            if (DrawingChar == value)
                return;
            
            SetValue(DrawingCharProperty, value);
        }
    }

    public static readonly DependencyProperty DrawingCharProperty =
        DependencyProperty.Register(
            nameof(DrawingChar),
            typeof(char),
            typeof(AsciiCanvas), 
            new PropertyMetadata('*'));

    public new FontFamily FontFamily
    {
        get => (FontFamily)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public static readonly new DependencyProperty FontFamilyProperty =
        DependencyProperty.Register(
        nameof(FontFamily),
        typeof(FontFamily),
        typeof(AsciiCanvas),
        new PropertyMetadata(new FontFamily("Consolas")));

    public int Rows
    {
        get => (int)GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    public static readonly DependencyProperty RowsProperty =
        DependencyProperty.Register(
            nameof(Rows),
            typeof(int),
            typeof(AsciiCanvas),
            new PropertyMetadata(0, OnCanvasSizeChanged));

    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register(
            nameof(Columns),
            typeof(int),
            typeof(AsciiCanvas),
            new PropertyMetadata(0, OnCanvasSizeChanged));

    public ObservableCollection<CharCell> Cells { get; }
           = new ObservableCollection<CharCell>();

    public double CellWidth => CanvasRepeater.ActualWidth / Columns;
    public double CellHeight => CanvasRepeater.ActualHeight / Rows;

    private Selection? _selection;
    private Point _lastCellPosition = new(-1, -1);
    private readonly ReadOnlyDictionary<DrawingTool, ToolHandler> _toolHandlers;
    private ToolHandler _currentToolHandler;
    private const char DefaultCharacter = ' ';

    public AsciiCanvas()
    {
        this.InitializeComponent();

        _toolHandlers = new ReadOnlyDictionary<DrawingTool, ToolHandler>(
            new Dictionary<DrawingTool, ToolHandler>
            {
                [DrawingTool.Pencil] = new PencilToolHandler(this),
                [DrawingTool.Eraser] = new EraserToolHandler(this),
                [DrawingTool.Selection] = new SelectionToolHandler(this),
                [DrawingTool.Pipette] = new PipetteToolHandler(this)
            });

        RegisterPropertyChangedCallback(DrawingToolProperty, (d, dp) =>
        {
            var canvas = (AsciiCanvas)d;
            canvas._currentToolHandler = canvas._toolHandlers[canvas.DrawingTool];
        });
    }

    private void InitializeCanvas()
    {
        if (Rows < 0 || Columns < 0)
            throw new ArgumentException("Rows and Columns must be positive");

        Cells.Clear();

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Columns; col++)
            {
                Cells.Add(new CharCell(col, row));
            }
        }
    }

    private static void OnCanvasSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as AsciiCanvas)?.InitializeCanvas();
    }

    private void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

    }

    private void OnCellPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var pointProps = e.GetCurrentPoint(this).Properties;
        if (!pointProps.IsLeftButtonPressed)
            return;

        var control = sender as ContentControl;
        if (control?.DataContext is not CharCell cell)
            return;

        _lastCellPosition = new Point(cell.Column, cell.Row);

        _currentToolHandler.HandlePointerPressed(cell, e);
    }

    private void OnCellPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        var control = sender as ContentControl;

        VisualStateManager.GoToState(control, "PointerOver", true);

        var pointerProps = e.GetCurrentPoint(this).Properties;

        if (!e.Pointer.IsInContact
            || !pointerProps.IsLeftButtonPressed
            || control?.DataContext is not CharCell cell
            || _lastCellPosition.X == cell.Column && _lastCellPosition.Y == cell.Row)
        {
            return;
        }

        _lastCellPosition = new Point(cell.Column, cell.Row);

        _currentToolHandler.HandlePointerEntered(cell, e);
    }

    private void OnCellPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is ContentControl control)
        {
            VisualStateManager.GoToState(control, "Normal", true);
        }
    }

    public void UpdateSelection(int column, int row)
    {
        if (!IsValidPoint(column, row))
            throw new ArgumentOutOfRangeException($"Columns and rows must be greater than or equal to zero and less than total columns: {Columns} and rows: {Rows}");

        if (_selection is null)
        {
            _selection = new Selection
            {
                Area = new Rectangle
                {
                    Fill = new SolidColorBrush
                    { 
                        Color = Color.FromArgb(50, 0, 0, 0),
                        Opacity = 0.3
                    },
                    IsHitTestVisible = false
                },
                InitialColumn = column,
                InitialRow = row,
                StartRow = column,
                StartColumn = row
            };

            SelectionLayer.Children.Add(_selection.Area);
        }

        int minColumn = Math.Min(_selection.InitialColumn, column);
        int maxColumn = Math.Max(_selection.InitialColumn, column);
        int minRow = Math.Min(_selection.InitialRow, row);
        int maxRow = Math.Max(_selection.InitialRow, row);

        double left = minColumn * CellWidth;
        double top = minRow * CellHeight;
        double width = (maxColumn - minColumn + 1) * CellWidth;
        double height = (maxRow - minRow + 1) * CellHeight;

        _selection.StartColumn = minColumn;
        _selection.StartRow = minRow;

        Canvas.SetLeft(_selection.Area, left);
        Canvas.SetTop(_selection.Area, top);
        _selection.Area.Width = width;
        _selection.Area.Height = height;
    }

    public void ClearSelection()
    {
        SelectionLayer.Children.Clear();
        _selection = null;
    }

    private CharCell GetCell(int column, int row)
    {
        if (!IsValidPoint(column, row))
            throw new ArgumentOutOfRangeException($"Columns and rows must be greater than or equal to zero and less than total columns: {Columns} and rows: {Rows}");

        return Cells[row * Columns + column];
    }

    private void CanvasRepeater_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;

        var flyout = FlyoutBase.GetAttachedFlyout(element);
        var options = new FlyoutShowOptions
        {
            Placement = FlyoutPlacementMode.RightEdgeAlignedBottom,
            ShowMode = FlyoutShowMode.Standard,
            Position = e.GetPosition(element)
        };

        flyout?.ShowAt(element, options);

    }

    private void OnCopyClick(object sender, RoutedEventArgs e)
    {
        if (_selection is null)
            return;

        var selectionEndColumn = (int)(_selection.Area.Width / CellWidth + _selection.StartColumn) - 1;
        var selectionEndRow = (int)(_selection.Area.Height / CellHeight + _selection.StartRow) - 1;

        var text = GetCharacters(_selection.StartColumn, _selection.StartRow, selectionEndColumn, selectionEndRow);

        var package = new DataPackage
        {
            RequestedOperation = DataPackageOperation.Copy
        };
        package.SetText(text);
        Clipboard.SetContent(package);
    }

    private async void OnPasteClick(object sender, RoutedEventArgs e)
    {
        if (_selection is null)
            return;

        var package = Clipboard.GetContent();
        if (!package.Contains(StandardDataFormats.Text))
            return;

        var pastingText = await package.GetTextAsync();
        if (pastingText is null)
            return;

        var selectedColumnsCount = (int)(_selection.Area.Width / CellWidth);
        var selectedRowsCount = (int)(_selection.Area.Height / CellHeight);

        PasteCharacters(_selection.StartColumn, _selection.StartRow, selectedColumnsCount, selectedRowsCount, pastingText);       
    }

    private void OnCutClick(object sender, RoutedEventArgs e)
    {
        if (_selection is null)
            return;

        var selectionEndColumn = (int)(_selection.Area.Width / CellWidth + _selection.StartColumn) - 1;
        var selectionEndRow = (int)(_selection.Area.Height / CellHeight + _selection.StartRow) - 1;

        var text = CutCharacters(_selection.StartColumn, _selection.StartRow, selectionEndColumn, selectionEndRow);

        var package = new DataPackage
        {
            RequestedOperation = DataPackageOperation.Move
        };
        package.SetText(text);
        Clipboard.SetContent(package);
    }

    private void PasteCharacters(int startColumn, int startRow, int columnsCount, int rowsCount, string text)
    {
        if (text is null)
            return;

        var lines = text.Split("\r\n");

        var totalRows = Math.Min(lines.Length, rowsCount);

        for (var rowOffset = 0; rowOffset < totalRows; rowOffset++)
        {
            var line = lines[rowOffset];
            var totalColumns = Math.Min(line.Length, columnsCount);

            for (var colOffset = 0; colOffset < totalColumns; colOffset++)
            {
                var col = startColumn + colOffset;
                var row = startRow + rowOffset;

                var cell = GetCell(col, row);
                if (cell is not null)
                {
                    cell.Character = line[colOffset];
                }
            }
        }
    }

    private string GetCharacters(int startColumn, int startRow, int endColumn, int endRow)
    {
        if (!(IsValidPoint(startColumn, startRow)
            && IsValidPoint(endColumn, endRow)))
        {
            throw new ArgumentOutOfRangeException($"Columns and rows must be greater than or equal to zero and less than total columns: {Columns} and rows: {Rows}");
        }

        var sb = new StringBuilder();

        for (var row = startRow; row <= endRow; row++)
        {
            for (var column = startColumn; column <= endColumn; column++)
            {
                var cell = GetCell(column, row);
                sb.Append(cell.Character);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private string CutCharacters(int startColumn, int startRow, int endColumn, int endRow)
    {
        if (!(IsValidPoint(startColumn, startRow)
            && IsValidPoint(endColumn, endRow)))
        {
            throw new ArgumentOutOfRangeException($"Columns and rows must be greater than or equal to zero and less than total columns: {Columns} and rows: {Rows}");
        }

        var sb = new StringBuilder();

        for (var row = startRow; row <= endRow; row++)
        {
            for (var column = startColumn; column <= endColumn; column++)
            {
                var cell = GetCell(column, row);
                sb.Append(cell.Character);
                cell.Character = DefaultCharacter;
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private bool IsValidPoint(int column, int row)
        => column >= 0 && column < Columns
           && row >= 0 && row < Rows;

}