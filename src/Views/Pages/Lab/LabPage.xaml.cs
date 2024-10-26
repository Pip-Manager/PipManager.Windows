using PipManager.Windows.ViewModels.Pages.Lab;
using Wpf.Ui.Abstractions.Controls;

namespace PipManager.Windows.Views.Pages.Lab;

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