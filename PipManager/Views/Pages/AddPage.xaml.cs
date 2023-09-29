using PipManager.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace PipManager.Views.Pages;

public partial class AddPage : INavigableView<AddViewModel>
{
    public AddViewModel ViewModel { get; }

    public AddPage(AddViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}