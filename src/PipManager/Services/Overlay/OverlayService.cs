using System.Collections.ObjectModel;
using PipManager.Models.Package;
using PipManager.ViewModels.Pages.Overlay;
using PipManager.ViewModels.Windows;
using PipManager.Views.Windows;

namespace PipManager.Services.Overlay;

public class OverlayService(OverlayViewModel overlayViewModel, MainWindowViewModel mainWindowViewModel): IOverlayService
{
    private void ShowOverlay()
    {
        overlayViewModel.IsOverlayVisible = true;
        mainWindowViewModel.IsTitleBarCoverageGridVisible = false;
    }
    
    public void ShowPackageUpdateOverlay(List<PackageUpdateItem> packageUpdates, System.Action callback)
    {
        overlayViewModel.ConfirmCallback = callback;
        overlayViewModel.PackageUpdateItems = new ObservableCollection<PackageUpdateItem>(packageUpdates);
        ShowOverlay();
    }
}
