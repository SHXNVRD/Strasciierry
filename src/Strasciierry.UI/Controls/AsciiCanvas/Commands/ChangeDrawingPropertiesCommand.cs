using System.Drawing;
using Strasciierry.Core.Commands;

namespace Strasciierry.UI.Controls.AsciiCanvas.Commands;

public class ChangeDrawingPropertiesCommand(IAsciiCanvas canvas, GraphicToolContext context) : ICommand
{
    private char? _oldCharacter;
    private Color? _oldForeground;
    private Color? _oldBackground;
    private FontStyle? _oldFontStyle;
    private FontFamily? _oldFontFamily;
    
    public Task Do()
    {
        if (context.PointerEvent != PointerEvent.Pressed)
            return Task.CompletedTask;

        var cell = canvas.GetCell(context.Column, context.Row);
        ChangeCanvasDrawingProperties(cell);

        return Task.CompletedTask;
    }

    public Task Undo()
    {
        if (_oldCharacter.HasValue
            && _oldForeground.HasValue
            && _oldBackground.HasValue
            && _oldFontStyle.HasValue
            && _oldFontFamily is not null)
        {
            var cell = new AsciiCanvasCell(0, 0)
            {
                Character = _oldCharacter.Value,
                Foreground = _oldForeground.Value,
                Background = _oldBackground.Value,
                FontStyle = _oldFontStyle.Value,
                FontFamily = _oldFontFamily
            };
            
            ChangeCanvasDrawingProperties(cell);
        }
        
        return Task.CompletedTask;
    }

    private void ChangeCanvasDrawingProperties(AsciiCanvasCell cell)
    {
        _oldCharacter = canvas.DrawingChar;
        _oldForeground = canvas.DrawingForeground;
        _oldBackground = canvas.DrawingBackground;
        _oldFontStyle = canvas.DrawingFontStyle;
        _oldFontFamily = new FontFamily(canvas.DrawingFontFamily.Name);
        
        canvas.ApplyDrawingPropertiesFromCell(cell);
    }
}