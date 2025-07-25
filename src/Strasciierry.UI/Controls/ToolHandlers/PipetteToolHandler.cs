﻿using Microsoft.UI.Xaml.Input;
using Strasciierry.UI.Controls.ToolHandlers.Base;

namespace Strasciierry.UI.Controls.ToolHandlers;

class PipetteToolHandler(AsciiCanvas canvas) : ToolHandler
{
    public override void HandlePointerPressed(CharCell cell, PointerRoutedEventArgs e)
    {
        canvas.DrawingChar = cell.Character;
    }
}
