using Wpf.Ui.Controls;
using Action_ActionViewModel = PipManager.Windows.ViewModels.Pages.Action.ActionViewModel;
using ActionViewModel = PipManager.Windows.ViewModels.Pages.Action.ActionViewModel;

namespace PipManager.Windows.Views.Pages.Action;

public partial class ActionPage : INavigableView<Action_ActionViewModel>
{
    public Action_ActionViewModel ViewModel { get; }

    public ActionPage(Action_ActionViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}