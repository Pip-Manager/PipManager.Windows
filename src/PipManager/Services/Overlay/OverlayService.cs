using PipManager.ViewModels.Pages.Overlay;

namespace PipManager.Services.Overlay;

public class OverlayService: IOverlayService
{
    private OverlayViewModel _overlayViewModel;
    public OverlayService(OverlayViewModel overlayViewModel)
    {
        _overlayViewModel = overlayViewModel;
    }
    public void ShowOverlay()
    {
        _overlayViewModel.IsOverlayVisible = true;
    }
}
