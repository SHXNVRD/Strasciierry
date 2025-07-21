using System.Collections.ObjectModel;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace Strasciierry.UI.Controls;

public sealed partial class CharacterPalette : UserControlBase
{
    public Character SelectedCharacter
    {
        get => (Character)GetValue(SelectedCharacterDependencyProperty);
        set => SetValue(SelectedCharacterDependencyProperty, value);
    }

    public static readonly DependencyProperty SelectedCharacterDependencyProperty =
        DependencyProperty.Register(
            nameof(SelectedCharacter),
            typeof(Character),
            typeof(CharacterPalette),
            new PropertyMetadata(null));

    private const string DefaultCharacterPalette = "`.-':_,^=;><+!rc*/z?sLTv)J7(|Fi{C}fI31tlu[neoZ5Yxjya]2ESwqkP6h9d4VpOGbUAKXHm8RD#$Bg0MNWQ%&@";

    private readonly double _itemSize = 30;
    private readonly double _itemSpacing = 2;
    private readonly double _itemsContainerPadding = 7;
    private readonly double _itemsContainerMinWidth;
    private readonly double _totalItemSize;

    private readonly ObservableCollection<Character> _characters = [.. DefaultCharacterPalette.Select(c => new Character(c))];

    private readonly int _columns = 6;
    private readonly double _thumbXOffset;
    private readonly double _thumbYOffset;

    public CharacterPalette()
    {
        InitializeComponent();
        Loaded += (s, e) => UpdateThumbPosition();
        SizeChanged += (s, e) => UpdateThumbPosition();

        _totalItemSize = _itemSize + _itemSpacing;
        _thumbXOffset = (_itemSize - Thumb.Width) / 2;
        _thumbYOffset = (_itemSize - Thumb.Height) / 2;
        _itemsContainerMinWidth = _itemSize * _columns + (_columns - 1) * _itemSpacing;
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

        ThumbTransform.X = column * _totalItemSize + _thumbXOffset;
        ThumbTransform.Y = row * _totalItemSize + _thumbYOffset;
    }

    private void ChangeItemsCount(int delta)
    {
        if (delta > 0)
        {
            for (var i = 0; i < delta; i++)
            {
                _characters.Add(new Character());
            }
        }
        else if (delta < 0)
        {
            var removeCount = Math.Min(-delta, _characters.Count);
            for (var i = 0; i < removeCount; i++)
            {
                _characters.RemoveAt(_characters.Count - 1);
            }
        }
    }

    /// <summary>
    /// Calculates the target number of elements based on the current coordinates
    /// </summary>
    /// <param name="thumbX">X coordinate</param>
    /// <param name="thumbY">Y coordinate</param>
    /// <returns>The maximum number of items that can be placed in ItemsContainer</returns>
    private int CalculateTargetItemCount(double thumbX, double thumbY)
    {
        var thumbCell = GetItemCell(thumbX, thumbY);
        var column = (int)thumbCell.X;
        var row = (int)thumbCell.Y;

        return row * _columns + column;
    }

    /// <summary>
    /// Returns the cell within which the x and y coordinates are located
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="x"/> less than 0 or greater than the ItemsContainer width
    /// or <paramref name="y"/> less than 0 or greater than the ItemsContainer height
    /// </exception>
    private Point GetItemCell(double x, double y)
    {
        if (x < 0 || x > ItemsContainer.Width)
            throw new ArgumentOutOfRangeException(nameof(x), x, $"Parameter {nameof(x)} must be greater than zero and less than the ItemsContainer width");
        if (y < 0 || y > ItemsContainer.Height)
            throw new ArgumentOutOfRangeException(nameof(y), y, $"Parameter {nameof(y)} must be greater than zero and less than the ItemsContainer height");

        var column = (int)Math.Floor(x / _totalItemSize);
        var row = (int)Math.Floor(y / _totalItemSize);

        return new Point(column, row);
    }

    private void ControlThumb_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        Thumb.CapturePointer(e.Pointer);
    }

    private void ControlThumb_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!e.Pointer.IsInContact)
            return;

        var point = e.GetCurrentPoint(LayoutRoot);
        var thumbX = point.Position.X;
        var thumbY = point.Position.Y;

        // Boundaries of movement
        thumbX = Math.Clamp(thumbX, 0, LayoutRoot.ActualWidth);
        thumbY = Math.Clamp(thumbY, 0, LayoutRoot.ActualHeight);

        var newCount = CalculateTargetItemCount(thumbX, thumbY);

        var delta = newCount - _characters.Count;

        if (delta != 0)
            ChangeItemsCount(delta);
    }

    private void ControlThumb_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        UpdateThumbPosition();
        ChangeCursor(InputSystemCursor.Create(InputSystemCursorShape.Arrow));
        Thumb.ReleasePointerCapture(e.Pointer);
    }

    private void ControlThumb_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        ChangeCursor(InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast));
    }

    private void ControlThumb_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (e.Pointer.IsInContact)
            return;

        ChangeCursor(InputSystemCursor.Create(InputSystemCursorShape.Arrow));
    }
}