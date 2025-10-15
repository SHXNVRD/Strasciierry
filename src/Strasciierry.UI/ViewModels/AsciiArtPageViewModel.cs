using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Automation;
using Strasciierry.Core;
using Strasciierry.Core.Extensions;
using Strasciierry.UI.Controls.AsciiCanvas;
using Strasciierry.UI.Controls.CharacterPalette;
using Strasciierry.UI.Extensions;
using Strasciierry.UI.Factories;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.ImageConverters;
using Strasciierry.UI.Services.Fonts;
using Strasciierry.UI.Services.Settings;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.AllJoyn;
using Windows.Devices.PointOfService.Provider;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Text;
using FontStyle = System.Drawing.FontStyle;

namespace Strasciierry.UI.ViewModels;

public partial class AsciiArtPageViewModel : ViewModelBase, IAsciiArtPageViewModel
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private SoftwareBitmap? _bitmap;

    private static Color _defaultArtBackground => Color.FromArgb(255, 50, 50, 50);
    private static Color _defaultArtForeground => Color.White;

    [ObservableProperty] public partial Color ArtBackground { get; set; } = _defaultArtBackground;

    [ObservableProperty] public partial Color ArtForeground { get; set; } = _defaultArtForeground;

    [ObservableProperty] public partial int SizePercent { get; set; } = 100;

    [ObservableProperty] public partial double HeightReductionFactor { get; set; } = 1;

    [ObservableProperty] public partial bool IsNegative { get; set; }

    [ObservableProperty] public partial GraphicTool GraphicTool { get; set; }

    [ObservableProperty] public partial CharacterPaletteItem SelectedItem { get; set; }

    [ObservableProperty] public partial int Columns { get; set; }

    [ObservableProperty] public partial int Rows { get; set; }

    public ObservableRangeCollection<AsciiCanvasCell> Cells { get; set; } = [];

    private readonly ISaveArtStrategyFactory _saveArtStrategyFactory;
    private readonly ILocalSettingsService _settingsService;

    public AsciiArtPageViewModel(
        ISaveArtStrategyFactory saveArtStrategyFactory,
        ILocalSettingsService settingsService)
    {
        _saveArtStrategyFactory = saveArtStrategyFactory;
        _settingsService = settingsService;
        InitializeCanvas(25, 15);
    }

    private void InitializeCanvas(int columns, int rows)
    {
        if (columns < 0 || rows < 0)
            throw new ArgumentException("Rows and Columns must be positive");

        Columns = columns;
        Rows = rows;
        Cells.Clear();

        var cells = new AsciiCanvasCell[columns * rows];

        for (var row = 0; row < Rows; row++)
        {
            for (var column = 0; column < Columns; column++)
            {
                cells[row * Columns + column] = new AsciiCanvasCell(column, row);
            }
        }

        Cells.AddRange(cells);
    }

    public async Task LoadImageAsync(StorageFile file)
    {
        if (file == null)
            return;

        try
        {
            _bitmap = await GetSoftwareBitmapAsync(file);
        }
        catch (Exception ex)
        {
            await DialogHelper.ShowErrorAsync(App.XamlRoot, $"{ex.Message}\n{ex.StackTrace}");
        }
    }

    public async Task LoadArtAsync(StorageFile file)
    {
        if (file == null)
            return;

        try
        {
            using var stream = await file.OpenReadAsync();
            using var streamReader = new StreamReader(stream.AsStream());
            var text = await streamReader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            await DialogHelper.ShowErrorAsync(App.XamlRoot, $"{ex.Message}\n{ex.StackTrace}");
        }
    }

    [RelayCommand]
    private async Task GenerateArtAsync()
    {
        try
        {
            if (_bitmap == null)
                return;

            var width = _bitmap.PixelWidth * SizePercent / 100;
            var height = (int)(_bitmap.PixelHeight / HeightReductionFactor * width / _bitmap.PixelWidth);
            using var resizedBitmap = _bitmap.Resize(width, height);
            using var grayScaleBitMap = resizedBitmap.ConvertToGrayscale();
            string art;

            if (IsNegative)
            {
                art = await Task.Run(() => ImageToSymbolsConverter.Convert(grayScaleBitMap, _settingsService.AppSettings.ImageToSymbolsSettings.ImageToSymbolsCharacters.Reverse()));
            }
            else
            {
                art = await Task.Run(() => ImageToSymbolsConverter.Convert(grayScaleBitMap, _settingsService.AppSettings.ImageToSymbolsSettings.ImageToSymbolsCharacters));
            }

            dispatcherQueue.TryEnqueue(() => SetArt(art, true));
        }
        catch (Exception ex)
        {
            await DialogHelper.ShowErrorAsync(App.XamlRoot, $"{ex.Message}\n{ex.StackTrace}");
        }
    }

    public async Task SaveArtAsync(StorageFile file)
    {
        if (file == null || string.IsNullOrWhiteSpace(file.Path))
            return;

        try
        {
            var saveStrategy = _saveArtStrategyFactory.CreateStrategy(file.FileType);
            await saveStrategy.SaveAsync(this, file);
        }
        // TODO: Подумать как перенести нотификейшн во вью
        catch (ExternalException ex)
        {
            await DialogHelper.ShowErrorAsync(App.XamlRoot, $"{ex.Message}\n{ex.StackTrace}");
        }
        catch (Exception ex)
        {
            await DialogHelper.ShowErrorAsync(App.XamlRoot, $"{ex.Message}\n{ex.StackTrace}");
        }
    }

    public Image DrawArt()
    {
        const float fontSize = 14f;
        const string fontName = "Consolas";

        var stringFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        using var supportImage = new Bitmap(1, 1);
        using var supportGraphics = Graphics.FromImage(supportImage);
        var supportFont = new Font(fontName, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);

        var symbolSize = supportGraphics.MeasureString("W", supportFont);
        supportFont.Dispose();
        var symbolWidth = (int)Math.Ceiling(symbolSize.Width);
        var symbolHeight = (int)Math.Ceiling(symbolSize.Height);

        var finalImage = new Bitmap(Columns * symbolWidth, Rows * symbolHeight);
        using var graphics = Graphics.FromImage(finalImage);

        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var fontCache = new Dictionary<FontStyle, Font>();

        try
        {
            foreach (var cell in Cells)
            {
                if (!fontCache.TryGetValue(cell.FontStyle, out var font))
                {
                    font = new Font(fontName, fontSize, cell.FontStyle, GraphicsUnit.Pixel);
                    fontCache.Add(cell.FontStyle, font);
                }

                int x = cell.Column * symbolWidth;
                int y = cell.Row * symbolHeight;
                var rect = new Rectangle(x, y, symbolWidth, symbolHeight);

                using (var backgroundBrush = new SolidBrush(cell.Background))
                {
                    graphics.FillRectangle(backgroundBrush, rect);
                }

                using (var foregroundBrush = new SolidBrush(cell.Foreground))
                {
                    graphics.DrawString(
                        cell.Symbol.ToString(),
                        font,
                        foregroundBrush,
                        rect,
                        stringFormat);
                }
            }
        }
        finally
        {
            stringFormat.Dispose();
            
            foreach (var font in fontCache.Values)
            {
                font.Dispose();
            }
        }

        return finalImage;
    }

    public string GetArt()
    {
        var sb = new StringBuilder(Columns * Rows);

        for (var row = 0; row < Rows; row++)
        {
            for (var column = 0; column < Columns; column++)
            {
                var cell = Cells[row * Columns + column];
                sb.Append(cell.Symbol);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public void SetArt(string art, bool resize = false)
    {
        if (string.IsNullOrEmpty(art))
            return;

        Cells.Clear();

        var artLines = art.Split(Environment.NewLine);

        if (resize)
            InitializeCanvas(artLines[0].Length, artLines.Length);

        var rows = Math.Min(Rows, artLines.Length);

        for (var row = 0; row < rows; row++)
        {
            var line = artLines[row];
            var columns = Math.Min(Columns, line.Length);

            for (var column = 0; column < columns; column++)
            {
                Cells[row * Columns + column].Symbol = line[column];
            }
        }
    }

    private async Task<SoftwareBitmap> GetSoftwareBitmapAsync(StorageFile file)
    {
        using var stream = await file.OpenAsync(FileAccessMode.Read);
        var decoder = await BitmapDecoder.CreateAsync(stream);
        return await decoder.GetSoftwareBitmapAsync();
    }
}