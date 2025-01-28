using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using Strasciierry.UI.Contracts.Services;
using Strasciierry.UI.Extensions;
using Microsoft.Extensions.Options;
using Strasciierry.UI.Options;
using Strasciierry.UI.ImageConverters;
using Strasciierry.Core.Extensions;
using Strasciierry.UI.Helpers;
using Microsoft.UI.Dispatching;
using System.Diagnostics;

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
    public partial string? SymbolicArt { get; set; }

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
            await Task.Run(() => GenerateArtProcessAsync());
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

        var newWidth = _softwareBitmap.PixelWidth * SizePercent / 100;
        var newHeight = _softwareBitmap.PixelHeight / HeightReductionFactor * newWidth / _softwareBitmap.PixelWidth;
        using var resizedBitmap = _softwareBitmap.Resize(newWidth, (int)newHeight);
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
            var file = await _filePickerService.PickSaveTxtAsync(App.MainWindow);

            if (file == null)
                return;

            await FileIO.WriteTextAsync(file, SymbolicArt);
        }
        catch (Exception ex)
        {
            await DialogHelper.ShowErrorAsync(App.Root.XamlRoot,  $"{ex.Message}\n{ex.StackTrace}");
        }
    }

    [RelayCommand]
    private void ResetSettings()
    {
        HeightReductionFactor = DefaultHeightReductionFactor;
        SizePercent = DefaultSizePercent;
        IsNegative = false;
        SymbolicArt = string.Empty;
        ImageFile = null;
    }

    public async Task<SoftwareBitmap> GetSoftwareBitmapAsync(StorageFile image)
    {
        using var stream = await image.OpenAsync(FileAccessMode.Read);
        var decoder = await BitmapDecoder.CreateAsync(stream);
        return await decoder.GetSoftwareBitmapAsync();
    }
}
