using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Storage;

namespace Strasciierry.UI.Contracts.Services;
public interface IFilePickerService
{
    Task<StorageFile> PickImageAsync(Window window);
    Task<StorageFile> PickSaveTxtAsync(Window window);
}
