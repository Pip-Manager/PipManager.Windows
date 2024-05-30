using System.Collections.ObjectModel;
using PipManager.Models.Package;
using PipManager.ViewModels.Pages.Overlay;
using PipManager.ViewModels.Windows;

namespace PipManager.Services.Overlay;

public class OverlayService(OverlayViewModel overlayViewModel, MainWindowViewModel mainWindowViewModel): IOverlayService
{
    private void ShowOverlay()
    {
        overlayViewModel.IsOverlayVisible = true;
        mainWindowViewModel.IsTitleBarCoverageGridVisible = true;
    }
    
    public void ShowPackageUpdateOverlay(List<PackageUpdateItem> packageUpdates, System.Action callback)
    {
        overlayViewModel.ConfirmCallback = callback;
        overlayViewModel.PackageUpdateItems = new ObservableCollection<PackageUpdateItem>(packageUpdates);
        ShowOverlay();
    }
}
