using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using EnvironmentViewModel = PipManager.Windows.ViewModels.Pages.Environment.EnvironmentViewModel;

namespace PipManager.Windows.Views.Pages.Environment;

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