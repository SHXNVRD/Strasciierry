using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Controls;

public sealed partial class GeneralSettingsControl : UserControlBase
{
    public GeneralSettingsControlViewModel ViewModel { get; }

    public GeneralSettingsControl(GeneralSettingsControlViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}
