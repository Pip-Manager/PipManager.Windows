using System.Collections.ObjectModel;
using PipManager.Windows.Models.Package;
using PipManager.Windows.ViewModels.Windows;

namespace PipManager.Windows.ViewModels.Pages.Overlay;

public partial class OverlayViewModel(MainWindowViewModel mainWindowViewModel): ObservableObject
{
    public System.Action? ConfirmCallback { get; set; }
    
    [ObservableProperty]
    private bool _isOverlayVisible;
    
    [RelayCommand]
    private void CloseOverlay()
    {
        IsOverlayVisible = false;
        mainWindowViewModel.IsTitleBarCoverageGridVisible = false;
    }

    [ObservableProperty] private ObservableCollection<PackageUpdateItem> _packageUpdateItems = [];

    [RelayCommand]
    private void Confirm()
    {
        CloseOverlay();
        ConfirmCallback?.Invoke();
    }
}
