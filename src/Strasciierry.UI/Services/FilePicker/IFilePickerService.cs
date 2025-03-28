using Microsoft.UI.Xaml;
using Windows.Storage;

namespace Strasciierry.UI.Services.FilePicker;
public interface IFilePickerService
{
    Task<StorageFile> PickImageAsync(Window window);
    Task<StorageFile> PickSaveAsync(Window window);
}
