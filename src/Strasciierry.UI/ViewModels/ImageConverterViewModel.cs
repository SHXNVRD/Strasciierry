﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Strasciierry.UI.Contracts.Services;
using Strasciierry.UI.Extensions;
using Strasciierry.Core.Extensions;
using Strasciierry.UI.Helpers;
using Microsoft.UI.Dispatching;
using Windows.ApplicationModel.DataTransfer;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace Strasciierry.UI.ViewModels;

public partial class ImageConverterViewModel : ObservableRecipient
{
    private readonly IFilePickerService _filePickerService;
    private readonly IUserSymbolsService _userSymbolsService;
    private readonly IImageToCharsService _imageToCharsService;
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private SoftwareBitmap? _softwareBitmap;

    private const int DefaultHeightReductionFactor = 1;
    private const int DefaultSizePercent = 100;

    [ObservableProperty]
    public partial Color ArtBackground { get; set; } = Color.Black;

    [ObservableProperty]
    public partial Color ArtForeground { get; set; } = Color.White;

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
    public partial bool IsNegative { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImagePath))]
    public partial StorageFile? ImageFile { get; set; }

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
        IUserSymbolsService userSymbolsService,
        IImageToCharsService imageToCharsService)
    {
        _filePickerService = pickerService;
        _userSymbolsService = userSymbolsService;
        _imageToCharsService = imageToCharsService;
    }

    [RelayCommand]
    private void CopyArt()
    {
        var package = new DataPackage();
        package.SetText(SymbolicArt);
        Clipboard.SetContent(package);
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
            await DialogHelper.ShowErrorAsync(App.Root.XamlRoot, $"{ex.Message}\n{ex.StackTrace}");
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
            await DialogHelper.ShowErrorAsync(App.Root.XamlRoot, $"{ex.Message}\n{ex.StackTrace}");
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

        if (_userSymbolsService.IsUserSymbolsOn)
        {
            if (IsNegative)
                rows = await _imageToCharsService.ConvertNegativeAsync(grayScaleBitMap, _userSymbolsService.UserSymbols);
            else
                rows = await _imageToCharsService.ConvertAsync(grayScaleBitMap, _userSymbolsService.UserSymbols);

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
            Image img;
            var file = await _filePickerService.PickSaveAsync(App.MainWindow);

            if (file == null || string.IsNullOrEmpty(file.Path))
                return;

            switch (file.FileType)
            {
                case ".png":
                    img = await Task.Run(() => DrawArt(file));
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Png));
                    break;
                case ".jpeg":
                    img = await Task.Run(() => DrawArt(file));
                    await Task.Run(() => img.Save(file.Path, ImageFormat.Jpeg));
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
            await DialogHelper.ShowErrorAsync(App.Root.XamlRoot,  $"{ex.Message}\n{ex.StackTrace}");
        }
    }

    private Image DrawArt(StorageFile file)
    {
        Image img = new Bitmap(1, 1);

        var drawing = Graphics.FromImage(img);
        var font = new Font("Consolas", 14, FontStyle.Regular, GraphicsUnit.Pixel);
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
        ArtForeground = Color.White;
        ArtBackground = Color.Black;
    }

    public async Task<SoftwareBitmap> GetSoftwareBitmapAsync(StorageFile image)
    {
        using var stream = await image.OpenAsync(FileAccessMode.Read);
        var decoder = await BitmapDecoder.CreateAsync(stream);
        return await decoder.GetSoftwareBitmapAsync();
    }
}
