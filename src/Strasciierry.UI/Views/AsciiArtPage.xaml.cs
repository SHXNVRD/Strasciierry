using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Strasciierry.UI.Controls.AsciiCanvas;
using Strasciierry.UI.Controls.CharacterPalette;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.ViewModels;
using Strasciierry.UI.Controls.AsciiCanvas.EventArguments;

namespace Strasciierry.UI.Views;

public sealed partial class AsciiArtPage : Page
{
    public AsciiArtPageViewModel ViewModel
    {
        get;
    }

    public ICommand OpenFileCommand { get; }
    public ICommand SaveFileCommand { get; }

    public AsciiArtPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<AsciiArtPageViewModel>();
        OpenFileCommand = new AsyncRelayCommand(OpenArtAsync);
        SaveFileCommand = new AsyncRelayCommand(SaveArtAsync);
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        AsciiCanvas.DrawingPropertiesChanged += AsciiCanvas_DrawingPropertiesChanged;
    }

    private void AsciiCanvas_DrawingPropertiesChanged(object? sender, DrawingPropertiesChangedEventArgs e)
    {
        var selectingItem = new CharacterPaletteItem
        {
            Character = e.Character,
            Foreground = e.Foreground,
            Background = e.Background,
            FontFamily = e.FontFamily,
            FontStyle = e.FontStyle
        };

        CharacterPalette.SelectOrAdd(selectingItem);
    }

    private async Task OpenArtAsync()
    {
        var file = await FilePickerHelper
            .CreateDocumentFileOpenPicker()
            .PickSingleFileAsync();

        if (file == null)
            return;

        FilePathTextBlock.Text = file.Path ?? string.Empty;

        await ViewModel.LoadArtAsync(file);
    }

    private async Task SaveArtAsync()
    {
        var file = await FilePickerHelper
            .CreateGenericFileSavePicker()
            .PickSaveFileAsync();

        if (file == null)
            return;

        await ViewModel.SaveArtAsync(file);
    }
}
