using Wpf.Ui.Abstractions.Controls;
using ActionExceptionViewModel = PipManager.Windows.ViewModels.Pages.Action.ActionExceptionViewModel;

namespace PipManager.Windows.Views.Pages.Action;

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