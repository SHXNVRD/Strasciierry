using Strasciierry.Core.Commands;

namespace Strasciierry.UI.Controls.AsciiCanvas.Commands;

public class GraphicToolCommandFactory : IGraphicToolCommandFactory
{
    public ICommand CreateCommand(IAsciiCanvas canvas, GraphicToolContext context, GraphicTool tool)
        => tool switch
        {
            GraphicTool.Eraser => new EraseCommand(canvas, context),
            GraphicTool.Pencil => new DrawCommand(canvas, context),
            GraphicTool.Pipette => new ChangeDrawingPropertiesCommand(canvas, context),
            GraphicTool.Selection => new SelectCommand(canvas, context),
            _ => throw new NotSupportedException("Unknown graphic tool type")
        };
}