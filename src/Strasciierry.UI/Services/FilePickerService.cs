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

namespace Strasciierry.UI.Services;
internal class FilePickerService : IFilePickerService
{
    private FilePickerOptions _options;
    private bool _initialized;

    private IList<string> _openFileTypes = new List<string>();
    private IDictionary<string, IList<string>> _saveFileTypes = new Dictionary<string, IList<string>>();
    private string _savingFileName;

    private readonly ReadOnlyCollection<string> _defaultOpenFileTypes = new(new[]
    {
        ".jpg",
        ".png",
        ".gif",
        ".exif",
        ".tiff"
    });

    private readonly Dictionary<string, IList<string>> _defaultSaveFileType = new(new[]
    {
        new KeyValuePair<string, IList<string>>("Plain text", new[] { ".txt" })
    });

    private const string DefaultSavingFileName = "New ASCII art file";

    public FilePickerService(IOptions<FilePickerOptions> options)
    {
        _options = options.Value;
    }

    public async Task<StorageFile> PickImageAsync(Window window)
    {
        await InitializeAsync();

        var openPicker = new FileOpenPicker();
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.Desktop;

        foreach (var openFileType in _openFileTypes)
            openPicker.FileTypeFilter.Add(openFileType);

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        return await openPicker.PickSingleFileAsync();
    }

    public async Task<StorageFile> PickSaveTxtAsync(Window window)
    {
        await InitializeAsync();

        FileSavePicker savePicker = new FileSavePicker();
        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        savePicker.SuggestedFileName = _savingFileName;

        foreach (var saveFileType in _saveFileTypes)
            savePicker.FileTypeChoices.Add(saveFileType);

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

        return await savePicker.PickSaveFileAsync();
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
