using System.Collections.ObjectModel;
using PipManager.Models.Package;
using PipManager.Views.Windows;

namespace PipManager.ViewModels.Pages.Overlay;

public partial class OverlayViewModel: ObservableObject
{
    public System.Action? ConfirmCallback { get; set; }
    
    [ObservableProperty]
    private bool _isOverlayVisible;
    
    [RelayCommand]
    private void CloseOverlay()
    {
        IsOverlayVisible = false;
        App.GetService<MainWindow>().TitleBarCoverageGrid.Visibility = Visibility.Collapsed;
    }

    [ObservableProperty] private ObservableCollection<PackageUpdateItem> _packageUpdateItems = [];

    [RelayCommand]
    private void Confirm()
    {
        CloseOverlay();
        ConfirmCallback?.Invoke();
    }
}
