using Wpf.Ui.Controls;
using EnvironmentViewModel = PipManager.ViewModels.Pages.Environment.EnvironmentViewModel;

namespace PipManager.Views.Pages.Environment;

public partial class EnvironmentPage : INavigableView<EnvironmentViewModel>
{
    public EnvironmentViewModel ViewModel { get; }

    public EnvironmentPage(EnvironmentViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}