using System.Windows.Controls;
using PipManager.ViewModels.Pages.Overlay;

namespace PipManager.Views.Pages.Overlay;

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
