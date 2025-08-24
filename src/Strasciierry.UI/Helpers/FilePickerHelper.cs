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
    public static KeyValuePair<string, IReadOnlyCollection<string>> Ico { get; } = new("Windows icon", [".ico"]);
    public static KeyValuePair<string, IReadOnlyCollection<string>> Wmf { get; } = new("Windows Metafile", [".wmf"]);
    public static KeyValuePair<string, IReadOnlyCollection<string>> Emf { get; } = new("Enhanced Metafile", [".emf"]);

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
}