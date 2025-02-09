using Avalonia.Controls;
using PipManager.Desktop.ViewModels.Pages;

namespace PipManager.Desktop.Views.Pages;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
    }
    
    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        InitializeComponent();
        DataContext = settingsViewModel;
    }
}