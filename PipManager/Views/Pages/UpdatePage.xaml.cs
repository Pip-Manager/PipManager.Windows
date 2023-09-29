using PipManager.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace PipManager.Views.Pages;

public partial class UpdatePage : INavigableView<UpdateViewModel>
{
    public UpdateViewModel ViewModel { get; }

    public UpdatePage(UpdateViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}