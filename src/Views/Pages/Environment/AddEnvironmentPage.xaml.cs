using PipManager.Windows.ViewModels.Pages.Environment;
using Wpf.Ui.Controls;

namespace PipManager.Windows.Views.Pages.Environment;

public partial class AddEnvironmentPage : INavigableView<AddEnvironmentViewModel>
{
    public AddEnvironmentViewModel ViewModel { get; }

    public AddEnvironmentPage(AddEnvironmentViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}