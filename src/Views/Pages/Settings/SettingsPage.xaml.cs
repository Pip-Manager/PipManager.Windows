using Wpf.Ui.Abstractions.Controls;
using SettingsViewModel = PipManager.Windows.ViewModels.Pages.Settings.SettingsViewModel;

namespace PipManager.Windows.Views.Pages.Settings;

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