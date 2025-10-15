using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Strasciierry.UI.Controls.AsciiCanvas;
using Strasciierry.UI.Controls.CharacterPalette;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.ViewModels;
using Strasciierry.UI.Controls.AsciiCanvas.EventArguments;
using Windows.Storage;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace Strasciierry.UI.Views;

public sealed partial class AsciiArtPage : Page
{
    public AsciiArtPageViewModel ViewModel
    {
        get;
    }

    public IRelayCommand OpenFileCommand { get; }
    public IRelayCommand SaveFileCommand { get; }
    public IRelayCommand ShowSettingsWindowCommand { get; }

    public AsciiArtPage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<AsciiArtPageViewModel>();
        OpenFileCommand = new AsyncRelayCommand(OpenArtAsync);
        SaveFileCommand = new AsyncRelayCommand(SaveArtAsync);
        ShowSettingsWindowCommand = new RelayCommand(ShowSettingsWindow);
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
            .CreateGenericFileOpenPicker()
            .PickSingleFileAsync();

        if (file == null)
            return;

        FilePathTextBlock.Text = file.Path ?? string.Empty;

        if (FilePickerHelper.IsDocument(file.FileType))
            await ViewModel.LoadArtAsync(file);
        else if (FilePickerHelper.IsImage(file.FileType))
        {
            SetPreviewSourceImage(file);
            await ViewModel.LoadImageAsync(file);
        }
    }

    private void SetPreviewSourceImage(StorageFile file)
    {
        if (!FilePickerHelper.IsImage(file.FileType))
            return;

        var uri = new Uri(file.Path);
        var image = new BitmapImage(uri);
        PreviewSourceImage.Source = image;
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

    private void ShowSettingsWindow()
        => WindowHelper.OpenWindow<SettingsWindow>();
}