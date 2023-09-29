using Wpf.Ui.Controls;
using SettingsViewModel = PipManager.ViewModels.Pages.Settings.SettingsViewModel;

namespace PipManager.Views.Pages.Settings;

public partial class SettingsPage : INavigableView<SettingsViewModel>
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}