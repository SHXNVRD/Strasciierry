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

namespace Strasciierry.UI.Services;

internal class FilePickerService : IFilePickerService
{
    private FilePickerOptions _options;
    private bool _initialized;

    private IList<string> _openFileTypes = new List<string>();
    private List<KeyValuePair<string, IList<string>>> _saveFileTypes = [];
    private string _savingFileName;

    private readonly ReadOnlyCollection<string> _defaultOpenFileTypes = new(new[]
    {
        ".jpg",
        ".png",
        ".gif",
        ".exif",
        ".tiff"
    });

    private readonly List<KeyValuePair<string, IList<string>>> _defaultSaveFileType = new(
    [
        new KeyValuePair<string, IList<string>>("Image", [".png"]),
        new KeyValuePair<string, IList<string>>("Plain text", [".txt"])
    ]);

    private const string DefaultSavingFileName = "Strasciierry";

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

            if (_options.OpenFileTypes != null && _options.OpenFileTypes.Count > 0)
            {
                foreach (var openFileType in _options.OpenFileTypes)
                    _openFileTypes.Add(openFileType);
            }
            else
                _openFileTypes = _defaultOpenFileTypes;

            if (_options.SaveFileTypes != null && _options.SaveFileTypes.Count > 0)
            {
                foreach (var saveFileType in _options.SaveFileTypes)
                    _saveFileTypes.Add(saveFileType);
            }
            else
                _saveFileTypes = _defaultSaveFileType;

            _savingFileName = _options.SavingFileName ?? DefaultSavingFileName;

            _initialized = true;
        });
    }
}
