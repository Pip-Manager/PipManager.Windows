using PipManager.ViewModels.Pages.Lab;
using PipManager.ViewModels.Pages.Search;
using Wpf.Ui.Controls;

namespace PipManager.Views.Pages.Lab;

public partial class LabPage : INavigableView<LabViewModel>
{
    public LabViewModel ViewModel { get; }

    public LabPage(LabViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}