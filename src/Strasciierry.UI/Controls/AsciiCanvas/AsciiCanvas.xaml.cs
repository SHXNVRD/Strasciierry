using System.Collections.ObjectModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers;
using Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers.Base;
using Windows.Foundation;

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
            if (DrawingChar != value)
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

    private Rectangle? _selectionRect;
    private Point _selectionStartPoint;
    private Point _lastCellPosition = new(-1, -1);
    private readonly ReadOnlyDictionary<DrawingTool, ToolHandler> _toolHandlers;
    private ToolHandler _currentToolHandler;

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

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
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

    public void UpdateSelection(int col, int row)
    {
        if (_selectionRect == null)
        {
            _selectionStartPoint = new Point(col, row);

            _selectionRect = new Rectangle
            {
                Fill = new SolidColorBrush(Colors.Blue)
                {
                    Opacity = 0.3
                },
                IsHitTestVisible = false
            };

            SelectionLayer.Children.Add(_selectionRect);
        }

        var startCol = (int)_selectionStartPoint.X;
        var startRow = (int)_selectionStartPoint.Y;

        int minCol = Math.Min(startCol, col);
        int maxCol = Math.Max(startCol, col);
        int minRow = Math.Min(startRow, row);
        int maxRow = Math.Max(startRow, row);

        double cellWidth = CanvasRepeater.ActualWidth / Columns;
        double cellHeight = CanvasRepeater.ActualHeight / Rows;

        double left = minCol * cellWidth;
        double top = minRow * cellHeight;
        double width = (maxCol - minCol + 1) * cellWidth;
        double height = (maxRow - minRow + 1) * cellHeight;

        Canvas.SetLeft(_selectionRect, left);
        Canvas.SetTop(_selectionRect, top);
        _selectionRect.Width = width;
        _selectionRect.Height = height;
    }

    public void ClearSelection()
    {
        SelectionLayer.Children.Clear();
        _selectionRect = null;
    }

    private CharCell? GetCell(Point position)
    {
        double cellWidth = CanvasRepeater.ActualWidth / Columns;
        double cellHeight = CanvasRepeater.ActualHeight / Rows;

        var col = (int)(position.X / cellWidth);
        var row = (int)(position.Y / cellHeight);

        if (col >= 0 && col < Columns && row >= 0 && row < Rows)
        {
            return Cells[row * Columns + col];
        }

        return null;
    }
}