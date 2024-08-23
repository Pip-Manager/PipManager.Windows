using Wpf.Ui.Controls;
using Action_ActionExceptionViewModel = PipManager.Windows.ViewModels.Pages.Action.ActionExceptionViewModel;
using ActionExceptionViewModel = PipManager.Windows.ViewModels.Pages.Action.ActionExceptionViewModel;

namespace PipManager.Windows.Views.Pages.Action;

public partial class ActionExceptionPage : INavigableView<Action_ActionExceptionViewModel>
{
    public Action_ActionExceptionViewModel ViewModel { get; }

    public ActionExceptionPage(Action_ActionExceptionViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}