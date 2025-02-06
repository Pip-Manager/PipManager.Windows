using Wpf.Ui.Abstractions.Controls;
using ActionViewModel = PipManager.Windows.ViewModels.Pages.Action.ActionViewModel;

namespace PipManager.Windows.Views.Pages.Action;

public partial class ActionPage : INavigableView<ActionViewModel>
{
    public ActionViewModel ViewModel { get; }

    public ActionPage(ActionViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}