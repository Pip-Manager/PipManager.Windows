using Wpf.Ui.Controls;
using About_AboutViewModel = PipManager.Windows.ViewModels.Pages.About.AboutViewModel;
using AboutViewModel = PipManager.Windows.ViewModels.Pages.About.AboutViewModel;

namespace PipManager.Windows.Views.Pages.About;

public partial class AboutPage : INavigableView<About_AboutViewModel>
{
    public About_AboutViewModel ViewModel { get; }

    public AboutPage(About_AboutViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}