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

namespace Strasciierry.UI.ViewModels;

public partial class ImageConverterViewModel : ObservableRecipient
{
    private const int Denominator = 100;

    private readonly IFilePickerService _filePickerService;
    private readonly IUserSymbolsService _userSymbolsService;
    private readonly IImageToCharsService _imageToCharsService;
    private SoftwareBitmap? _softwareBitmap;

    [ObservableProperty]
    public partial string? SymbolicArt { get; set; }

    [ObservableProperty]
    public partial int ArtSizePercent { get; set; } = 100;

    [ObservableProperty]
    public partial double WidthOffset { get; set; } = 1;

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
        ImageFile = await _filePickerService.PickImageAsync(App.MainWindow);

        if (ImageFile == null)
            return;

        _softwareBitmap = await GetSoftwareBitmapAsync(ImageFile);
    }

    [RelayCommand]
    private async Task OnGenerateASCII()
    {
        if (_softwareBitmap == null)
            return;

        try
        {
            var newWidth = _softwareBitmap.PixelWidth * ArtSizePercent / Denominator;
            var newHeight = _softwareBitmap.PixelHeight / WidthOffset * newWidth / _softwareBitmap.PixelHeight;
            using var resizedBitmap = _softwareBitmap.Resize(newWidth, (int)newHeight);
            using var grayScaleBitMap = resizedBitmap.ConvertToGrayscale();
            char[][] rows;

            if (_userSymbolsService.UseUserSymbols)
            {
                if (IsNegative)
                    rows = await _imageToCharsService.ConvertNegativeAsync(grayScaleBitMap, _userSymbolsService.UserSymbols);
                else
                    rows = await _imageToCharsService.ConvertAsync(grayScaleBitMap, _userSymbolsService.UserSymbols);

                SymbolicArt = rows.Stringify();
            }
            else
            {
                if (IsNegative)
                    rows = await _imageToCharsService.ConvertNegativeAsync(grayScaleBitMap);
                else
                    rows = await _imageToCharsService.ConvertAsync(grayScaleBitMap);

                SymbolicArt = rows.Stringify();
            }

        }
        catch (Exception ex)
        {
            await DialogHelper.ShowgAsync(App.Root.XamlRoot, "Во время преобразования возникла ошибка", $"{ex.Message}\n{ex.StackTrace}", "ОК");
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SymbolicArt == null)
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
            await DialogHelper.ShowgAsync(App.Root.XamlRoot, "Во время сохранения возникла ошибка", $"{ex.Message}\n{ex.StackTrace}", "ОК");
        }
    }

    [RelayCommand]
    private void ResetSettings()
    {
        WidthOffset = 1;
        ArtSizePercent = 100;
        IsNegative = false;
    }

    public async Task<SoftwareBitmap> GetSoftwareBitmapAsync(StorageFile image)
    {
        using var stream = await image.OpenAsync(FileAccessMode.Read);
        var decoder = await BitmapDecoder.CreateAsync(stream);
        return await decoder.GetSoftwareBitmapAsync();
    }
}
