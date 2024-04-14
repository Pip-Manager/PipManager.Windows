using System.Collections.ObjectModel;
using PipManager.Models.Package;
using PipManager.Views.Windows;

namespace PipManager.ViewModels.Pages.Overlay;

public partial class OverlayViewModel: ObservableObject
{
    private System.Action? _confirmCallback;
    
    [ObservableProperty]
    private bool _isOverlayVisible;
    
    [RelayCommand]
    private void CloseOverlay()
    {
        IsOverlayVisible = false;
        App.GetService<MainWindow>().TitleBarCoverageGrid.Visibility = Visibility.Collapsed;
    }
    
    private void ShowOverlay()
    {
        IsOverlayVisible = true;
        App.GetService<MainWindow>().TitleBarCoverageGrid.Visibility = Visibility.Visible;
    }

    [ObservableProperty] private ObservableCollection<PackageUpdateItem> _packageUpdateItems = [];

    public void ShowPackageUpdateOverlay(List<PackageUpdateItem> packageUpdates, System.Action callback)
    {
        _confirmCallback = callback;
        PackageUpdateItems = new ObservableCollection<PackageUpdateItem>(packageUpdates);
        ShowOverlay();
    }

    [RelayCommand]
    private void Confirm()
    {
        CloseOverlay();
        _confirmCallback?.Invoke();
    }
}
