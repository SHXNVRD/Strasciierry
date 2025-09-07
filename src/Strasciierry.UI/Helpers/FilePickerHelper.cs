using System.Collections.ObjectModel;
using Strasciierry.Core.Exceptions;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Strasciierry.UI.Helpers;

public static class FilePickerHelper
{
    public static KeyValuePair<string, IReadOnlyCollection<string>> Png { get; } = new("PNG", [".png"]);
    public static KeyValuePair<string, IReadOnlyCollection<string>> Jpg { get; } = new("JPG", [".jpg", ".jpeg"]);
    public static KeyValuePair<string, IReadOnlyCollection<string>> Bmp { get; } = new("BMP", [".bmp"]);
    public static KeyValuePair<string, IReadOnlyCollection<string>> Gif { get; } = new("GIF", [".gif"]);
    public static KeyValuePair<string, IReadOnlyCollection<string>> Tiff { get; } = new("TIFF", [".tiff"]);
    public static KeyValuePair<string, IReadOnlyCollection<string>> Webp { get; } = new("WEBP", [".webp"]);
    public static KeyValuePair<string, IReadOnlyCollection<string>> Heif { get; } = new("HEIF", [".heif"]);
    public static KeyValuePair<string, IReadOnlyCollection<string>> Ico { get; } = new("ICO", [".ico"]);
    public static KeyValuePair<string, IReadOnlyCollection<string>> Wmf { get; } = new("WMF", [".wmf"]);
    public static KeyValuePair<string, IReadOnlyCollection<string>> Emf { get; } = new("EMF", [".emf"]);

    public static KeyValuePair<string, IReadOnlyCollection<string>> Txt { get; } = new("Plain text", [".txt"]);
    
    public static IReadOnlyCollection<KeyValuePair<string, IReadOnlyCollection<string>>> ImageAll { get; } = 
    [
        Png,
        Jpg,
        Bmp,
        Gif,
        Tiff,
        Webp,
        Heif,
        Ico,
        Wmf,
        Emf
    ];
    
    public static IReadOnlyCollection<KeyValuePair<string, IReadOnlyCollection<string>>> DocumentAll { get; } =
    [
        Txt
    ];

    public static IReadOnlyCollection<KeyValuePair<string, IReadOnlyCollection<string>>> ExtensionsAll { get; } =
        [.. ImageAll, .. DocumentAll];

    public static bool IsImage(string fileName)
    {
        var extension = Path.GetExtension(fileName);

        if (extension is null)
            return false;

        return ImageAll.Any(i => i.Value.Contains(extension));
    }

    public static bool IsDocument(string fileName)
    {
        var extension = Path.GetExtension(fileName);

        if ( extension is null) 
            return false;

        return DocumentAll.Any(d => d.Value.Contains(extension));
    }

    public static FileSavePicker CreateImageFileSavePicker()
    {
        var filePicker = CreateFileSavePicker(ImageAll);
        filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

        return filePicker;
    }

    public static FileOpenPicker CreateImageFileOpenPicker()
    {
        var extensions = ImageAll.SelectMany(e => e.Value);
        var filePicker = CreateFileOpenPicker(extensions);
        filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        
        return filePicker;
    }

    public static FileSavePicker CreateDocumentFileSavePicker()
    {
        var filePicker = CreateFileSavePicker(DocumentAll);
        filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

        return filePicker;
    }

    public static FileOpenPicker CreateDocumentFileOpenPicker()
    {
        var extensions = DocumentAll.SelectMany(e => e.Value);
        var filePicker = CreateFileOpenPicker(extensions);
        filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        
        return filePicker;
    }

    public static FileOpenPicker CreateGenericFileOpenPicker()
    {
        var extensions = new List<string>();
        extensions.AddRange(ImageAll.SelectMany(e => e.Value));
        extensions.AddRange(DocumentAll.SelectMany(e => e.Value));

        var filePicker = CreateFileOpenPicker(extensions);
        filePicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;

        return filePicker;
    }

    public static FileSavePicker CreateGenericFileSavePicker()
    {
        var extensions = ImageAll.Concat(DocumentAll);

        var filePicker = CreateFileSavePicker(extensions);
        filePicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;

        return filePicker;
    }

    private static FileOpenPicker CreateFileOpenPicker(IEnumerable<string> extensions)
    {
        var filePicker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail
        };

        foreach (var fileType in extensions)
        {
            filePicker.FileTypeFilter.Add(fileType);
        }
        
        var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(filePicker, hWnd);

        return filePicker;
    }
    
    private static FileSavePicker CreateFileSavePicker(IEnumerable<KeyValuePair<string, IReadOnlyCollection<string>>> extensions)
    {
        var filePicker = new FileSavePicker
        {
            SuggestedFileName = "ascii-art"
        };
        
        foreach (var fileType in extensions)
        {
            filePicker.FileTypeChoices.Add(fileType.Key, [..fileType.Value]);
        }

        var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(filePicker, hWnd);

        return filePicker;
    }

    public static string GetFileFormat(string fileName)
    {
        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(extension))
            throw new ArgumentException("File extension cannot be null or am empty string");

        var format = ExtensionsAll.FirstOrDefault(e => e.Value.Contains(extension));

        if (format.Equals(default(KeyValuePair<string, IReadOnlyCollection<string>>)))
            throw new FileFormatNotSupportedException(extension);

        return format.Key;
    }
}