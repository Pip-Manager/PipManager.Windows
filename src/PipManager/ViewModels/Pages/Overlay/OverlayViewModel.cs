using PipManager.Views.Pages.Overlay;

namespace PipManager.ViewModels.Pages.Overlay;

public partial class OverlayViewModel : ObservableObject
{
    [ObservableProperty]
    private int _testValue;
    
    [ObservableProperty]
    private bool _isOverlayVisible;
    
    [RelayCommand]
    private void CloseOverlay()
    {
        IsOverlayVisible = false;
    }
}
