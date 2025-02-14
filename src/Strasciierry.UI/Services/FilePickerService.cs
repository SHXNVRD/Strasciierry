using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using Windows.Storage;
using Strasciierry.UI.Contracts.Services;
using Strasciierry.UI.Options;
using Microsoft.UI.Xaml.Controls;
using Windows.Security.Isolation;
using System.Security.Cryptography.X509Certificates;
using Windows.Storage.Provider;

namespace Strasciierry.UI.Services;

internal class FilePickerService : IFilePickerService
{
    private FilePickerOptions _options;

    private bool _initialized;

    private string _savingFileName;
    private readonly ReadOnlyCollection<string> _openFileTypes = new(
    [
        ".jpg",
        ".png",
        ".gif",
        ".exif",
        ".tiff"
    ]);
    private readonly List<KeyValuePair<string, IList<string>>> _saveFileTypes = new(
    [
        new KeyValuePair<string, IList<string>>("PNG", [".png"]),
        new KeyValuePair<string, IList<string>>("JPEG", [".jpeg"]),
        new KeyValuePair<string, IList<string>>("BMP", [".bmp"]),
        new KeyValuePair<string, IList<string>>("GIF", [".gif"]),
        new KeyValuePair<string, IList<string>>("TIFF", [".tiff"]),
        new KeyValuePair<string, IList<string>>("WEBP", [".webp"]),
        new KeyValuePair<string, IList<string>>("HEIF", [".heif"]),
        new KeyValuePair<string, IList<string>>("Windows icon", [".ico"]),
        new KeyValuePair<string, IList<string>>("Windows Metafile", [".wmf"]),
        new KeyValuePair<string, IList<string>>("Enhanced Metafile", [".emf"]),
        new KeyValuePair<string, IList<string>>("Text", [".txt"])
    ]);

    private const string DefaultSaveFileName = "SymbolicArt";

    public FilePickerService(IOptions<FilePickerOptions> options)
    {
        _options = options.Value;
    }

    public async Task<StorageFile> PickImageAsync(Window window)
    {
        await InitializeAsync();

        var openPicker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.PicturesLibrary
        };

        foreach (var openFileType in _openFileTypes)
            openPicker.FileTypeFilter.Add(openFileType);

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        return await openPicker.PickSingleFileAsync();
    }

    public async Task<StorageFile> PickSaveAsync(Window window)
    {
        await InitializeAsync();

        FileSavePicker picker = new()
        {
            SuggestedFileName = _savingFileName,
            SuggestedStartLocation = PickerLocationId.PicturesLibrary
        };

        foreach (var saveFileType in _saveFileTypes)
            picker.FileTypeChoices.Add(saveFileType);

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

        return await picker.PickSaveFileAsync();
    }

    private async Task InitializeAsync()
    {
        await Task.Run(() =>
        {
            if (_initialized)
                return;

            _savingFileName = string.IsNullOrWhiteSpace(_options.SaveFileName) ? DefaultSaveFileName : _options.SaveFileName;

            _initialized = true;
        });
    }
}
