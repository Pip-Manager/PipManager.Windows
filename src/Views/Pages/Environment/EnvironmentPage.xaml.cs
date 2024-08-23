using Wpf.Ui.Controls;
using Environment_EnvironmentViewModel = PipManager.Windows.ViewModels.Pages.Environment.EnvironmentViewModel;
using EnvironmentViewModel = PipManager.Windows.ViewModels.Pages.Environment.EnvironmentViewModel;

namespace PipManager.Windows.Views.Pages.Environment;

public partial class EnvironmentPage : INavigableView<Environment_EnvironmentViewModel>
{
    public Environment_EnvironmentViewModel ViewModel { get; }

    public EnvironmentPage(Environment_EnvironmentViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}