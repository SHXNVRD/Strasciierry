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
using Windows.System;

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
        set => SetValue(DrawingCharProperty, value);
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
                [DrawingTool.Eraser] = new EraserToolHandler(),
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

    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        //if (_isUpdatingLayout)
        //    return;

        //// ѕровер€ем существенное изменение размера
        //if (Math.Abs(e.NewSize.Width - _lastProcessedSize.Width) < 1 &&
        //    Math.Abs(e.NewSize.Height - _lastProcessedSize.Height) < 1)
        //{
        //    return;
        //}

        //if (CanvasRepeater?.Layout is UniformGridLayout layout)
        //{
        //    double totalWidth = CanvasRepeater.ActualWidth;
        //    double totalHeight = CanvasRepeater.ActualHeight;

        //    // »спользуем защитные проверки
        //    if (Columns < 1 || Rows < 1 || totalWidth <= 0 || totalHeight <= 0)
        //    {
        //        _isUpdatingLayout = false;
        //        return;
        //    }

        //    double cellWidth = totalWidth / Columns;
        //    double cellHeight = totalHeight / Rows;

        //    // ќграничение минимального размера
        //    cellWidth = Math.Max(1, cellWidth);
        //    cellHeight = Math.Max(1, cellHeight);

        //    // ѕримен€ем только при существенном изменении
        //    if (Math.Abs(layout.MinItemWidth - cellWidth) > 0.1 ||
        //        Math.Abs(layout.MinItemHeight - cellHeight) > 0.1)
        //    {
        //        layout.MinItemWidth = cellWidth;
        //        layout.MinItemHeight = cellHeight;
        //    }

        //    _lastProcessedSize = e.NewSize;
        //    _isUpdatingLayout = true;
        //}
    }

    private void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

    }

    private void OnCellPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var border = sender as Border;
        if (border?.DataContext is not CharCell cell)
            return;

        _lastCellPosition = new Point(cell.Column, cell.Row);

        _currentToolHandler.HandlePointerPressed(cell, e);
    }

    private void OnCellPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (!e.Pointer.IsInContact)
            return;

        var border = sender as Border;
        if (border?.DataContext is not CharCell cell)
            return;

        if (_lastCellPosition.X == cell.Column && _lastCellPosition.Y == cell.Row)
            return;

        _lastCellPosition = new Point(cell.Column, cell.Row);

        _currentToolHandler.HandlePointerEntered(cell, e);
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