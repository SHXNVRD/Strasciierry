using System.Collections.ObjectModel;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace Strasciierry.UI.Controls;

public sealed partial class CharacterPalette : UserControlBase
{
    public CharacterPaletteItem SelectedItem
    {
        get => (CharacterPaletteItem)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(
            nameof(SelectedItem),
            typeof(CharacterPaletteItem),
            typeof(CharacterPalette),
            new PropertyMetadata(null));

    private readonly double _itemSize = 30;
    private readonly double _itemSpacing = 2;
    private readonly double _itemsContainerPadding = 7;
    private readonly double _itemsContainerMinWidth;
    private readonly double _totalItemSize;

    private readonly int _columns = 6;
    private readonly double _thumbXOffset;
    private readonly double _thumbYOffset;
    private ScrollView _scrollView;

    private const string DefaultCharacterPalette = "`.-':_,^=;><+!rc*/z?sLTv)J7(|Fi{C}fI31tlu[neoZ5Yxjya]2ESwqkP6h9d4VpOGbUAKXHm8RD#$Bg0MNWQ%&@";

    private readonly ObservableCollection<CharacterPaletteItem> _characters = [.. DefaultCharacterPalette.Select(c => new CharacterPaletteItem(c))];

    public CharacterPalette()
    {
        InitializeComponent();
        Loaded += OnLoaded;

        _totalItemSize = _itemSize + _itemSpacing;
        _thumbXOffset = (_itemSize - Thumb.Width) / 2;
        _thumbYOffset = (_itemSize - Thumb.Height) / 2;
        _itemsContainerMinWidth = _itemSize * _columns + (_columns - 1) * _itemSpacing;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RegisterPropertyChangedCallback(SelectedItemProperty, SelectedItemPropertyChanged);
        _scrollView = ItemsContainer.ScrollView;
        _scrollView.ViewChanged += OnScrollViewChanged;

        SelectedItem = _characters[0];
        UpdateThumbPosition();
    }

    private void OnScrollViewChanged(ScrollView sender, object args)
    {
        UpdateThumbPosition();
    }

    private void UpdateThumbPosition()
    {
        var column = 0;
        var row = 0;

        if (_characters.Count != 0)
        {
            column = _characters.Count % _columns;
            row = _characters.Count / _columns;
        }

        var thumbNewXPosition = column * _totalItemSize + _thumbXOffset;
        var thumbNewYPosition = row * _totalItemSize + _thumbYOffset;
        
        var scrollOffset = _scrollView.ScrollableHeight - _scrollView.VerticalOffset;
        var maxVisiblePosition = ItemsContainer.ActualHeight - _totalItemSize + _thumbYOffset;

        thumbNewYPosition = Math.Clamp(thumbNewYPosition, 0, maxVisiblePosition) + scrollOffset;

        ThumbTransform.X = thumbNewXPosition;
        ThumbTransform.Y = thumbNewYPosition;
    }

    /// <summary>
    /// Calculates the target number of elements based on the current coordinates
    /// </summary>
    /// <param name="thumbX">X coordinate</param>
    /// <param name="thumbY">Y coordinate</param>
    /// <returns>The maximum number of items that can be placed in ItemsContainer</returns>
    private int CalculateTargetItemCount(double thumbX, double thumbY)
    {
        var thumbCell = CoordinatesToCell(thumbX, thumbY);
        var column = (int)thumbCell.X;
        var row = (int)thumbCell.Y;

        return row * _columns + column;
    }

    private Point CoordinatesToCell(double x, double y)
    {
        x = Math.Max(0, x);
        y = Math.Max(0, y);

        var column = (int)Math.Floor(x / _totalItemSize);
        var row = (int)Math.Floor(y / _totalItemSize);

        return new Point(column, row);
    }

    private void ControlThumb_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        Thumb.CapturePointer(e.Pointer);
    }

    private void ControlThumb_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint(LayoutRoot);
        var thumbX = point.Position.X;
        var thumbY = point.Position.Y + ItemsContainer.ScrollView.VerticalOffset;

        // Boundaries of horizontal movement
        thumbX = Math.Clamp(thumbX, 0, LayoutRoot.ActualWidth);

        var newCount = CalculateTargetItemCount(thumbX, thumbY);

        var delta = newCount - _characters.Count;

        if (delta > 0)
            AddCharacters(delta);
        if (delta < 0)
            RemoveCharacters(-delta);

        UpdateThumbPosition();
        SetCursor(InputSystemCursor.Create(InputSystemCursorShape.Arrow));
        Thumb.ReleasePointerCapture(e.Pointer);
    }

    public void AddCharacters(int count)
    {
        if (count <= 0)
            return;

        for (var i = 0; i < count; i++)
        {
            _characters.Add(new CharacterPaletteItem());
        }
    }

    public void RemoveCharacters(int count)
    {
        if (count <= 0)
            return;

        var removeCount = Math.Min(count, _characters.Count - 1);
        var lastSafeIndex = _characters.Count - removeCount - 1;

        if (_characters.IndexOf(SelectedItem) >= _characters.Count - removeCount)
            SelectedItem = _characters[lastSafeIndex];

        for (var i = 0; i < removeCount; i++)
        {
            _characters.RemoveAt(_characters.Count - 1);
        }
    }

    private void ControlThumb_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        SetCursor(InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast));
    }

    private void ControlThumb_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (e.Pointer.IsInContact)
            return;

        SetCursor(InputSystemCursor.Create(InputSystemCursorShape.Arrow));
    }

    private void ItemsContainer_SelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        SelectedItem = sender.SelectedItem as CharacterPaletteItem;
    }

    private void SelectedItemPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (dp != SelectedItemProperty)
            return;

        var index = _characters.IndexOf(SelectedItem);
        ItemsContainer.Select(index);

        if (index == _characters.Count - 1)
            ItemsContainer.StartBringItemIntoView(index, new BringIntoViewOptions { VerticalAlignmentRatio = 1f });
        else
            ItemsContainer.StartBringItemIntoView(index, new BringIntoViewOptions());
    }

    private async void ItemsContainer_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        var dialog = App.GetService<CharacterPaletteItemEditDialog>();
        dialog.EditingItem = (CharacterPaletteItem)args.InvokedItem;
        dialog.XamlRoot = App.XamlRoot;
        await dialog.ShowAsync();
    }
}