using System.Collections.ObjectModel;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers;
using Windows.ApplicationModel.DataTransfer;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using FontStyle = System.Drawing.FontStyle;
using Point = Windows.Foundation.Point;
using Rectangle = Microsoft.UI.Xaml.Shapes.Rectangle;

namespace Strasciierry.UI.Controls.AsciiCanvas;

public sealed partial class AsciiCanvas : UserControlBase, IAsciiCanvas
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
        set => SetValue(DrawingCharProperty, value);
    }

    public static readonly DependencyProperty DrawingCharProperty =
        DependencyProperty.Register(
            nameof(DrawingChar),
            typeof(char),
            typeof(AsciiCanvas),
            new PropertyMetadata('*'));

    public Color DrawingForeground
    {
        get => (Color)GetValue(DrawingForegroundProperty);
        set => SetValue(DrawingForegroundProperty, value);
    }

    public static readonly DependencyProperty DrawingForegroundProperty =
        DependencyProperty.Register(
            nameof(DrawingForeground),
            typeof(Color),
            typeof(AsciiCanvas),
            new PropertyMetadata(DefaultDrawingForeground));

    public Color DrawingBackground
    {
        get => (Color)GetValue(DrawingBackgroundProperty);
        set => SetValue(DrawingBackgroundProperty, value);
    }

    public static readonly DependencyProperty DrawingBackgroundProperty =
        DependencyProperty.Register(
            nameof(DrawingBackground),
            typeof(Color),
            typeof(AsciiCanvas),
            new PropertyMetadata(DefaultDrawingBackground));

    public FontStyle DrawingFontStyle
    {
        get => (FontStyle)GetValue(DrawingFontStyleProperty);
        set => SetValue(DrawingFontStyleProperty, value);
    }

    public static readonly DependencyProperty DrawingFontStyleProperty =
        DependencyProperty.Register(
            nameof(DrawingFontStyle),
            typeof(FontStyle),
            typeof(AsciiCanvas),
            new PropertyMetadata(DefaultDrawingFontStyle));

    public FontFamily DrawingFontFamily
    {
        get => (FontFamily)GetValue(DrawingFontFamilyProperty);
        set => SetValue(DrawingFontFamilyProperty, value);
    }

    public static readonly DependencyProperty DrawingFontFamilyProperty =
        DependencyProperty.Register(
            nameof(DrawingFontFamily),
            typeof(FontFamily),
            typeof(AsciiCanvas),
            new PropertyMetadata(DefaultDrawingFontFamily));

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

    public ObservableCollection<AsciiCanvasCell> Cells { get; }
           = new ObservableCollection<AsciiCanvasCell>();

    public double CellWidth => CanvasRepeater.ActualWidth / Columns;
    public double CellHeight => CanvasRepeater.ActualHeight / Rows;

    public static char DefaultCellCharacter => ' ';
    public static Color DefaultDrawingForeground => Color.White;
    public static Color DefaultDrawingBackground => Color.Transparent;
    public static FontFamily DefaultDrawingFontFamily => new("Consolas");
    public static FontStyle DefaultDrawingFontStyle => System.Drawing.FontStyle.Regular;

    public Selection Selection { get; private set; }
    private Rectangle _selectionRect;
    private Point _lastCellPosition = new(-1, -1);
    private readonly ReadOnlyDictionary<DrawingTool, ToolHandler> _toolHandlers;

    public event EventHandler<DrawingPropertiesChangedEventArgs>? DrawingPropertiesChanged;

    public AsciiCanvas()
    {
        InitializeComponent();

        _selectionRect = new Rectangle
        {
            Fill = new SolidColorBrush
            {
                Color = Windows.UI.Color.FromArgb(50, 0, 0, 0),
                Opacity = 0.3
            },
            IsHitTestVisible = false
        };

        SelectionLayer.Children.Add(_selectionRect);

        _toolHandlers = new ReadOnlyDictionary<DrawingTool, ToolHandler>(
            new Dictionary<DrawingTool, ToolHandler>
            {
                [DrawingTool.Pencil] = new PencilToolHandler(this),
                [DrawingTool.Eraser] = new EraserToolHandler(this),
                [DrawingTool.Selection] = new SelectionToolHandler(this),
                [DrawingTool.Pipette] = new PipetteToolHandler(this)
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
                Cells.Add(new AsciiCanvasCell(col, row));
            }
        }
    }

    private static void OnCanvasSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as AsciiCanvas)?.InitializeCanvas();
    }

    private void OnCellPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var pointProps = e.GetCurrentPoint(this).Properties;
        if (!pointProps.IsLeftButtonPressed)
            return;

        var control = sender as ContentControl;
        if (control?.DataContext is not AsciiCanvasCell cell)
            return;

        _lastCellPosition = new Point(cell.Column, cell.Row);

        var context = new ToolHandlerContext
        {
            PointerEvent = PointerEvent.Pressed,
            PointerEventArgs = e,
            CellRow = cell.Row,
            CellColumn = cell.Column
        };

        HandleTool(context);
    }

    private void OnCellPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        var control = sender as ContentControl;

        VisualStateManager.GoToState(control, "PointerOver", true);

        var pointerProps = e.GetCurrentPoint(this).Properties;

        if (!e.Pointer.IsInContact
            || !pointerProps.IsLeftButtonPressed
            || control?.DataContext is not AsciiCanvasCell cell
            || _lastCellPosition.X == cell.Column && _lastCellPosition.Y == cell.Row)
        {
            return;
        }

        _lastCellPosition = new Point(cell.Column, cell.Row);

        var context = new ToolHandlerContext
        {
            PointerEvent = PointerEvent.Entered,
            PointerEventArgs = e,
            CellRow = cell.Row,
            CellColumn = cell.Column
        };

        HandleTool(context);
    }

    private void OnCellPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is ContentControl control)
        {
            VisualStateManager.GoToState(control, "Normal", true);
        }
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
        if (_selectionRect is null)
            return;

        var selectionEndColumn = Selection.Columns + Selection.StartColumn - 1;
        var selectionEndRow = Selection.Rows + Selection.StartRow - 1;

        var text = GetCharacters(Selection.StartColumn, Selection.StartRow, selectionEndColumn, selectionEndRow);

        var package = new DataPackage
        {
            RequestedOperation = DataPackageOperation.Copy
        };
        package.SetText(text);
        Clipboard.SetContent(package);
    }

    private async void OnPasteClick(object sender, RoutedEventArgs e)
    {
        if (_selectionRect is null)
            return;

        var package = Clipboard.GetContent();
        if (!package.Contains(StandardDataFormats.Text))
            return;

        var pastingText = await package.GetTextAsync();
        if (pastingText is null)
            return;

        PasteCharacters(Selection.StartColumn, Selection.StartRow, Selection.Columns, Selection.Rows, pastingText);
    }

    private void OnCutClick(object sender, RoutedEventArgs e)
    {
        if (_selectionRect is null)
            return;

        var selectionEndColumn = Selection.Columns + Selection.StartColumn - 1;
        var selectionEndRow = Selection.Rows + Selection.StartRow - 1;

        var text = CutCharacters(Selection.StartColumn, Selection.StartRow, selectionEndColumn, selectionEndRow);

        var package = new DataPackage
        {
            RequestedOperation = DataPackageOperation.Move
        };
        package.SetText(text);
        Clipboard.SetContent(package);
    }

    public void SetSelection(Selection selection)
    {
        Selection = selection;

        double left = selection.StartColumn * CellWidth;
        double top = selection.StartRow * CellHeight;
        double width = selection.Columns * CellWidth;
        double height = selection.Rows * CellHeight;

        Canvas.SetLeft(_selectionRect, left);
        Canvas.SetTop(_selectionRect, top);
        _selectionRect.Width = width;
        _selectionRect.Height = height;
    }

    public void ClearSelection()
        => SetSelection(new Selection());

    public AsciiCanvasCell GetCell(int column, int row)
    {
        ValidateCell(column, row);

        return Cells[row * Columns + column];
    }

    public AsciiCanvasCell GetDefaultCell()
        => new(0, 0)
        {
            Character = DefaultCellCharacter,
            Foreground = DefaultDrawingForeground,
            Background = DefaultDrawingBackground,
            FontFamily = DefaultDrawingFontFamily,
            FontStyle = DefaultDrawingFontStyle
        };

    public AsciiCanvasCell GetStyledCell()
        => new(0, 0)
        {
            Character = DrawingChar,
            Foreground = DrawingForeground,
            Background = DrawingBackground,
            // FontFamily = new FontFamily(DrawingFontFamily.Name); ????
            FontFamily = DrawingFontFamily,
            FontStyle = DrawingFontStyle
        };

    private void PasteCharacters(int startColumn, int startRow, int columnsCount, int rowsCount, string text)
    {
        ValidateCell(startColumn, startRow);

        if (columnsCount < 0)
            throw new ArgumentException($"Columns count must be greater than zero", nameof(columnsCount));
        if (rowsCount < 0)
            throw new ArgumentException($"Rows count must be greater than zero", nameof(rowsCount));
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
        ValidateCellRange(startColumn, startRow, endColumn, endRow);

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

    public void ApplyDrawingPropertiesFromCell(AsciiCanvasCell cell)
    {
        DrawingChar = cell.Character;
        DrawingForeground = cell.Foreground;
        DrawingBackground = cell.Background;
        DrawingFontFamily = cell.FontFamily;
        DrawingFontStyle = cell.FontStyle;

        var eventArgs = new DrawingPropertiesChangedEventArgs(
            DrawingChar,
            DrawingForeground,
            DrawingBackground,
            DrawingFontFamily,
            DrawingFontStyle);

        DrawingPropertiesChanged?.Invoke(this, eventArgs);
    }

    private string CutCharacters(int startColumn, int startRow, int endColumn, int endRow)
    {
        ValidateCellRange(startColumn, startRow, endColumn, endRow);

        var sb = new StringBuilder();

        for (var row = startRow; row <= endRow; row++)
        {
            for (var column = startColumn; column <= endColumn; column++)
            {
                var cell = GetCell(column, row);
                sb.Append(cell.Character);
                cell.Character = DefaultCellCharacter;
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private void ValidateCell(int column, int row)
    {
        if (column < 0 || column > Columns)
            throw new ArgumentOutOfRangeException(nameof(column), column, $"Column must be in range [0, {Columns - 1}]");
        if (row < 0 || row > Columns)
            throw new ArgumentOutOfRangeException(nameof(row), row, $"Row must be in range [0, {Rows - 1}]");
    }

    private void ValidateCellRange(int startColumn, int startRow, int endColumn, int endRow)
    {
        ValidateCell(startColumn, startRow);
        ValidateCell(endColumn, endRow);

        if (startColumn > endColumn)
            throw new ArgumentException($"Start column ({startColumn}) cannot be greater than end column ({endColumn})", nameof(startColumn));
        if (startRow > endRow)
            throw new ArgumentException($"Start row ({startRow}) cannot be greater than end row ({endRow})", nameof(startRow));
    }

    private void HandleTool(ToolHandlerContext context)
        => _toolHandlers[DrawingTool].Handle(context);
}