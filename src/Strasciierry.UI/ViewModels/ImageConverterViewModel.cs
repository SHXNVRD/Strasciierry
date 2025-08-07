using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Strasciierry.Core.Extensions;
using Strasciierry.UI.Extensions;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.Services.FilePicker;
using Strasciierry.UI.Services.Fonts;
using Strasciierry.UI.Services.ImageToChars;
using Strasciierry.UI.Services.Settings;
using Strasciierry.UI.Services.UsersSymbols;
using Strasciierry.UI.Controls.AsciiCanvas;
using Strasciierry.UI.Controls.CharacterPalette;

namespace Strasciierry.UI.ViewModels;

public partial class ImageConverterViewModel : ViewModelBase
{
    private readonly IFilePickerService _filePickerService;
    private readonly IUsersSymbolsService _userSymbolsService;
    private readonly IImageToCharsService _imageToCharsService;
    private readonly IFontsService _fontsService;
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private SoftwareBitmap? _softwareBitmap;

    private const int DefaultHeightReductionFactor = 1;
    private const int DefaultSizePercent = 100;
    private const int DefaultFontSize = 14;
    private const string DefaultFontName = "Consolas";

    private static Color _defaultArtBackground => Color.FromArgb(255, 50, 50, 50);
    private static Color _defaultArtForeground => Color.White;

    public readonly ReadOnlyCollection<int> FontSizes = new([8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72]);

    public ObservableCollection<string> FilteredFonts => new (_fontsService.GetFonts().Select(f => f.FontFamily.Name));

    [ObservableProperty]
    public partial Color ArtBackground { get; set; } = _defaultArtBackground;

    [ObservableProperty]
    public partial Color ArtForeground { get; set; } = _defaultArtForeground;

    [ObservableProperty]
    public partial string? SymbolicArt { get; set; }

    [ObservableProperty]
    public partial int Width { get; set; }

    [ObservableProperty]
    public partial int Heigh { get; set; }

    [ObservableProperty]
    public partial int SizePercent { get; set; } = 100;

    [ObservableProperty]
    public partial double HeightReductionFactor { get; set; } = 1;

    [ObservableProperty]
    public partial int FontSize { get; set; } = DefaultFontSize;

    [ObservableProperty]
    public partial string FontName { get; set; } = DefaultFontName;

    [ObservableProperty]
    public partial FontStyle FontStyle { get; set; } = FontStyle.Regular;

    [ObservableProperty]
    public partial FontStyle FontWeight { get; set; } = FontStyle.Regular;

    [ObservableProperty]
    public partial FontStyle TextDecorations { get; set; } = FontStyle.Regular;

    [ObservableProperty]
    public partial bool IsNegative { get; set; }

    [ObservableProperty]
    public partial bool IsBoldChecked { get; set; }

    [ObservableProperty]
    public partial bool IsItalicChecked { get; set; }

    [ObservableProperty]
    public partial bool IsUnderlineChecked { get; set; }

    [ObservableProperty]
    public partial bool IsStrikeThroughChecked { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImagePath))]
    public partial StorageFile? ImageFile { get; set; }

    [ObservableProperty]
    public partial DrawingTool DrawingTool { get; set; }

    [ObservableProperty]
    public partial CharacterPaletteItem SelectedItem { get; set; }

    public string? ImagePath
    {
        get
        {
            if (ImageFile != null)
                return ImageFile.Path;
            else
                return default;
        }
    }

    public ImageConverterViewModel(
        IFilePickerService pickerService,
        IUsersSymbolsService userSymbolsService,
        IImageToCharsService imageToCharsService,
        ILocalSettingsService localSettingsService,
        IFontsService fontsService)
    {
        _filePickerService = pickerService;
        _userSymbolsService = userSymbolsService;
        _imageToCharsService = imageToCharsService;
        _fontsService = fontsService;
    }

    [RelayCommand]
    private void CopyArt()
    {
        var package = new DataPackage();
        package.SetText(SymbolicArt);
        Clipboard.SetContent(package);
    }

    [RelayCommand]
    private async Task OnOpenFileAsync()
    {

    }

    [RelayCommand]
    private async Task OnSaveFileAsync()
    {

    }

    [RelayCommand]
    private async Task OnAddAsync()
    {
        try
        {
            ImageFile = await _filePickerService.PickImageAsync(App.MainWindow);

            if (ImageFile == null)
                return;

            _softwareBitmap = await GetSoftwareBitmapAsync(ImageFile);
        }
        catch (Exception ex)
        {
            await DialogHelper.ShowErrorAsync(App.XamlRoot, $"{ex.Message}\n{ex.StackTrace}");
        }
    }

    [RelayCommand]
    private async Task OnGenerateArtAsync()
    {
        try
        {
            await GenerateArtProcessAsync();
        }
        catch (Exception ex)
        {
            await DialogHelper.ShowErrorAsync(App.XamlRoot, $"{ex.Message}\n{ex.StackTrace}");
        }
    }

    private async Task GenerateArtProcessAsync()
    {
        if (_softwareBitmap == null)
            return;

        Width = _softwareBitmap.PixelWidth * SizePercent / 100;
        Heigh = (int)(_softwareBitmap.PixelHeight / HeightReductionFactor * Width / _softwareBitmap.PixelWidth);
        using var resizedBitmap = _softwareBitmap.Resize(Width, Heigh);
        using var grayScaleBitMap = resizedBitmap.ConvertToGrayscale();
        char[][] rows;

        if (_userSymbolsService.UsersSymbolsOn)
        {
            if (IsNegative)
                rows = await _imageToCharsService.ConvertNegativeAsync(grayScaleBitMap, _userSymbolsService.UsersSymbols);
            else
                rows = await _imageToCharsService.ConvertAsync(grayScaleBitMap, _userSymbolsService.UsersSymbols);

            dispatcherQueue.TryEnqueue(() =>
            {
                SymbolicArt = rows.Stringify();
            });
        }
        else
        {
            if (IsNegative)
                rows = await _imageToCharsService.ConvertNegativeAsync(grayScaleBitMap);
            else
                rows = await _imageToCharsService.ConvertAsync(grayScaleBitMap);

            dispatcherQueue.TryEnqueue(() =>
            {
                SymbolicArt = rows.Stringify();
            });
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(SymbolicArt))
            return;

        try
        {
            var file = await _filePickerService.PickSaveAsync(App.MainWindow);

            if (file == null || string.IsNullOrEmpty(file.Path))
                return;

            var img = await Task.Run(() => DrawArt());

            switch (file.FileType)
            {
                case ".png":
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Png));
                    break;
                case ".jpeg":
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Jpeg));
                    break;
                case ".bmp":
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Bmp));
                    break;
                case ".gif":
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Gif));
                    break;
                case ".tiff":
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Tiff));
                    break;
                case ".webp":
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Webp));
                    break;
                case ".heif":
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Heif));
                    break;
                case ".ico":
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Icon));
                    break;
                case ".wmf":
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Wmf));
                    break;
                case ".emf":
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Emf));
                    break;
                case ".txt":
                    await FileIO.WriteTextAsync(file, SymbolicArt);
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            await DialogHelper.ShowErrorAsync(App.XamlRoot,  $"{ex.Message}\n{ex.StackTrace}");
        }
    }

    private Image DrawArt()
    {
        Image img = new Bitmap(1, 1);

        var drawing = Graphics.FromImage(img);
        var font = new Font(FontName, FontSize, FontStyle | FontWeight | TextDecorations, GraphicsUnit.Pixel);
        var imageSize = new SizeF(Width * font.Size, Heigh * font.Height);
        var textSize = drawing.MeasureString(SymbolicArt, font, imageSize);

        var sf = new StringFormat
        {
            Trimming = StringTrimming.Word
        };

        img.Dispose();
        drawing.Dispose();

        img = new Bitmap((int)textSize.Width, (int)textSize.Height);
        
        drawing = Graphics.FromImage(img);
        drawing.Clear(ArtBackground);
        drawing.CompositingQuality = CompositingQuality.Default;
        drawing.InterpolationMode = InterpolationMode.Default;
        drawing.PixelOffsetMode = PixelOffsetMode.Default;
        drawing.SmoothingMode = SmoothingMode.Default;
        drawing.TextRenderingHint = TextRenderingHint.SystemDefault;

        using var textBrush = new SolidBrush(ArtForeground);
        drawing.DrawString(SymbolicArt, font, textBrush, new RectangleF(0, 0, textSize.Width, textSize.Height), sf);
        drawing.Save();
        drawing.Dispose();

        return img;
    }

    [RelayCommand]
    private void ResetSettings()
    {
        HeightReductionFactor = DefaultHeightReductionFactor;
        SizePercent = DefaultSizePercent;
        IsNegative = false;
        SymbolicArt = string.Empty;
        ImageFile = null;
        Heigh = 0;
        Width = 0;
        ArtBackground = _defaultArtBackground;
        ArtForeground = _defaultArtForeground;
        FontStyle = FontStyle.Regular;
        FontWeight = FontStyle.Regular;
        TextDecorations = FontStyle.Regular;
        FontName = DefaultFontName;
        FontSize = DefaultFontSize;
        IsBoldChecked = false;
        IsItalicChecked = false;
        IsUnderlineChecked = false;
        IsStrikeThroughChecked = false;
    }

    public async Task<SoftwareBitmap> GetSoftwareBitmapAsync(StorageFile image)
    {
        using var stream = await image.OpenAsync(FileAccessMode.Read);
        var decoder = await BitmapDecoder.CreateAsync(stream);
        return await decoder.GetSoftwareBitmapAsync();
    }
}
