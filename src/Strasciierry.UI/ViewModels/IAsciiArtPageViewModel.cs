using System.Drawing;

namespace Strasciierry.UI.ViewModels;

public interface IAsciiArtPageViewModel
{
    Image DrawArt();
    string GetArt();
}