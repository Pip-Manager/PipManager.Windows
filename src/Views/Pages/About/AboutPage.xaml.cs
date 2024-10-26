using Wpf.Ui.Abstractions.Controls;
using AboutViewModel = PipManager.Windows.ViewModels.Pages.About.AboutViewModel;

namespace PipManager.Windows.Views.Pages.About;

public partial class AboutPage : INavigableView<AboutViewModel>
{
    public AboutViewModel ViewModel { get; }

    public AboutPage(AboutViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}