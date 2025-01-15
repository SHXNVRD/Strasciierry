using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strasciierry.UI.Options;
internal class FilePickerOptions
{
    public IList<string> OpenFileTypes { get; set; }
    public Dictionary<string, IList<string>> SaveFileTypes { get; set; }
    public string SavingFileName { get; set; }
}
