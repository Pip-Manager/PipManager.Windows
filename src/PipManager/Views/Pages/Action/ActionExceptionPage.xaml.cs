using Wpf.Ui.Controls;
using ActionExceptionViewModel = PipManager.ViewModels.Pages.Action.ActionExceptionViewModel;

namespace PipManager.Views.Pages.Action;

public partial class ActionExceptionPage : INavigableView<ActionExceptionViewModel>
{
    public ActionExceptionViewModel ViewModel { get; }

    public ActionExceptionPage(ActionExceptionViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}