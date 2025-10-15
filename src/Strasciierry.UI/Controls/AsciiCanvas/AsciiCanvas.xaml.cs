using System.Collections.ObjectModel;
using System.Data.Common;
using System.Drawing;
using System.Text;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Strasciierry.UI.Controls.AsciiCanvas.Commands;
using Strasciierry.UI.Controls.AsciiCanvas.EventArguments;
using Strasciierry.UI.Helpers;
using Windows.ApplicationModel.DataTransfer;
using static System.Net.Mime.MediaTypeNames;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using FontStyle = System.Drawing.FontStyle;
using Point = Windows.Foundation.Point;
using Rectangle = Microsoft.UI.Xaml.Shapes.Rectangle;

namespace Strasciierry.UI.Controls.AsciiCanvas;

public sealed partial class AsciiCanvas : UserControlBase, IAsciiCanvas
{
    public GraphicTool GraphicTool
    {
        get => (GraphicTool)GetValue(GraphicToolProperty);
        set => SetValue(GraphicToolProperty, value);
    }

    public static readonly DependencyProperty GraphicToolProperty =
        DependencyProperty.Register(
            nameof(GraphicTool),
            typeof(GraphicTool),
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
            new PropertyMetadata(0));

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
            new PropertyMetadata(0));

    public IList<AsciiCanvasCell> ItemsSource
    {
        get => (IList<AsciiCanvasCell>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IList<AsciiCanvasCell>),
            typeof(AsciiCanvas),
            new PropertyMetadata(null));

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

    public event EventHandler<DrawingPropertiesChangedEventArgs>? DrawingPropertiesChanged;

    private readonly IGraphicToolCommandFactory _commandFactory;

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

        _commandFactory = Ioc.Default.GetRequiredService<IGraphicToolCommandFactory>();
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

        var context = new GraphicToolContext(
            PointerEvent.Pressed,
            e,
            cell.Column,
            cell.Row);

        HandleGraphicTool(context);
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

        var context = new GraphicToolContext(
            PointerEvent.Entered,
            e,
            cell.Column,
            cell.Row);

        HandleGraphicTool(context);
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

    private async void OnCopyClick(object sender, RoutedEventArgs e)
    {
        if (_selectionRect is null)
            return;

        var cells = CopyCells(Selection.StartColumn, Selection.StartRow, Selection.EndColumn, Selection.EndRow);
        await ClipboardHelper.SetAsync(cells, ClipboardHelper.AsciiCanvasCellDataFormat, 3);
    }

    private async void OnPasteClick(object sender, RoutedEventArgs e)
    {
        if (_selectionRect is null)
            return;

        var cells = await ClipboardHelper.GetAsync<IEnumerable<AsciiCanvasCell>>(ClipboardHelper.AsciiCanvasCellDataFormat);
        if (cells is not null)
        {
            PasteSymbols(Selection.StartColumn, Selection.StartRow, Selection.Columns, Selection.Rows, [.. cells]);
        }
        else
        {
            var symbols = await ClipboardHelper.GetTextAsync();
            if (string.IsNullOrEmpty(symbols))
                return;

            PasteSymbols(Selection.StartColumn, Selection.StartRow, Selection.Columns, Selection.Rows, symbols);
        }
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

        return ItemsSource[row * Columns + column];
    }

    public AsciiCanvasCell GetDefaultCell()
        => new(0, 0)
        {
            Symbol = DefaultCellCharacter,
            Foreground = DefaultDrawingForeground,
            Background = DefaultDrawingBackground,
            FontFamily = new FontFamily(DrawingFontFamily.Name),
            FontStyle = DefaultDrawingFontStyle
        };

    public AsciiCanvasCell GetStyledCell()
        => new(0, 0)
        {
            Symbol = DrawingChar,
            Foreground = DrawingForeground,
            Background = DrawingBackground,
            FontFamily = new FontFamily(DrawingFontFamily.Name),
            FontStyle = DrawingFontStyle
        };

    private void PasteSymbols(int startColumn, int startRow, int columnsCount, int rowsCount, string text)
    {
        ValidateCell(startColumn, startRow);

        if (columnsCount < 0)
            throw new ArgumentException($"Columns count must be greater than zero", nameof(columnsCount));
        if (rowsCount < 0)
            throw new ArgumentException($"Rows count must be greater than zero", nameof(rowsCount));
        if (string.IsNullOrEmpty(text))
            throw new ArgumentNullException(nameof(text));

        var lines = text.Split(Environment.NewLine);
        var totalRows = Math.Min(lines.Length, rowsCount);
        var pastingCell = GetDefaultCell();

        for (var rowOffset = 0; rowOffset < totalRows; rowOffset++)
        {
            var line = lines[rowOffset];
            var totalColumns = Math.Min(line.Length, columnsCount);

            for (var colOffset = 0; colOffset < totalColumns; colOffset++)
            {
                var col = startColumn + colOffset;
                var row = startRow + rowOffset;
                pastingCell.Symbol = line[colOffset];

                GetCell(col, row).Update(pastingCell);
            }
        }
    }

    private void PasteSymbols(int startColumn, int startRow, int columnsCount, int rowsCount, AsciiCanvasCell[] cells)
    {
        ValidateCell(startColumn, startRow);

        if (columnsCount < 0)
            throw new ArgumentException($"Columns count must be greater than zero", nameof(columnsCount));
        if (rowsCount < 0)
            throw new ArgumentException($"Rows count must be greater than zero", nameof(rowsCount));
        if (cells is null)
            throw new ArgumentNullException(nameof(cells));
        if (!cells.Any())
            return;

        var minCol = cells.Min(c => c.Column);
        var minRow = cells.Min(c => c.Row);
        var maxCol = cells.Max(c => c.Column);
        var maxRow = cells.Max(c => c.Row);

        int sourceColumns = maxCol - minCol + 1;
        int sourceRows = maxRow - minRow + 1;

        var totalColumns = Math.Min(sourceColumns, columnsCount);
        var totalRows = Math.Min(sourceRows, rowsCount);

        for (var rowOffset = 0; rowOffset < totalRows; rowOffset++)
        {
            for (var colOffset = 0; colOffset < totalColumns; colOffset++)
            {
                var col = startColumn + colOffset;
                var row = startRow + rowOffset;
                var index = rowOffset * sourceColumns + colOffset;

                GetCell(col, row).Update(cells[index]);
            }
        }
    }

    private IEnumerable<AsciiCanvasCell> CopyCells(int startColumn, int startRow, int endColumn, int endRow)
    {
        ValidateCellRange(startColumn, startRow, endColumn, endRow);

        var cells = new List<AsciiCanvasCell>();

        for (var row = startRow; row <= endRow; row++)
        {
            for (var column = startColumn; column <= endColumn; column++)
            {
                var cell = GetCell(column, row).Clone();
                cells.Add(cell);
            }
        }

        return cells;
    }

    public void ApplyDrawingPropertiesFromCell(AsciiCanvasCell cell)
    {
        DrawingChar = cell.Symbol;
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
                sb.Append(cell.Symbol);
                cell.Symbol = DefaultCellCharacter;
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private void ValidateCell(int column, int row)
    {
        if (column < 0 || column > Columns)
            throw new ArgumentOutOfRangeException(nameof(column), column, $"Column must be in range [0, {Columns - 1}]");
        if (row < 0 || row > Rows)
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

    private void HandleGraphicTool(GraphicToolContext context)
    {
        var command = _commandFactory.CreateCommand(this, context, GraphicTool);
        command.Do();
    }
}

public enum GraphicTool
{
    Pencil,
    Eraser,
    Selection,
    Pipette
}

public record GraphicToolContext(
    PointerEvent PointerEvent,
    PointerRoutedEventArgs PointerEventArgs,
    int Column,
    int Row);