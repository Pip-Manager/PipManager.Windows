using PipManager.Windows.ViewModels.Pages.Overlay;

namespace PipManager.Windows.Views.Pages.Overlay;

public partial class OverlayPage
{
    public OverlayViewModel ViewModel { get; }

    public OverlayPage()
    {
        ViewModel = App.GetService<OverlayViewModel>();
        DataContext = this;
        InitializeComponent();
    }
}
