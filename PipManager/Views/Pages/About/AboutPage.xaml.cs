using PipManager.ViewModels.Pages;
using Wpf.Ui.Controls;
using AboutViewModel = PipManager.ViewModels.Pages.About.AboutViewModel;

namespace PipManager.Views.Pages.About;

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