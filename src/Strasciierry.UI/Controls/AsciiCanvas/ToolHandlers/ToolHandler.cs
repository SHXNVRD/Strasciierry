namespace Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers;

public abstract class ToolHandler
{
    protected readonly IAsciiCanvas canvas;

    protected ToolHandler(IAsciiCanvas canvas)
    {
        this.canvas = canvas;
    }

    public abstract void Handle(ToolHandlerContext context);
}
