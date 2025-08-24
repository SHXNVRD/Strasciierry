using Strasciierry.Core.Commands;

namespace Strasciierry.UI.Controls.AsciiCanvas.Commands;

public interface IGraphicToolCommandFactory
{
    ICommand CreateCommand(IAsciiCanvas canvas, GraphicToolContext context, GraphicTool tool);
}