using Microsoft.UI.Xaml.Input;

namespace Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers;

class PipetteToolHandler : ToolHandler
{
    public PipetteToolHandler(AsciiCanvas canvas) 
        : base(canvas)
    { }

    public override void Handle(ToolHandlerContext context)
    {
        if (context.PointerEvent != PointerEvent.Pressed)
            return;

        var cell = canvas.GetCell(context.CellColumn, context.CellRow);
        canvas.ApplyDrawingPropertiesFromCell(cell);
    }
}
