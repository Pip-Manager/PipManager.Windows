using Wpf.Ui.Controls;
using Settings_SettingsViewModel = PipManager.Windows.ViewModels.Pages.Settings.SettingsViewModel;
using SettingsViewModel = PipManager.Windows.ViewModels.Pages.Settings.SettingsViewModel;

namespace PipManager.Windows.Views.Pages.Settings;

public partial class SettingsPage : INavigableView<Settings_SettingsViewModel>
{
    public Settings_SettingsViewModel ViewModel { get; }

    public SettingsPage(Settings_SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}