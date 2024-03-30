using Wpf.Ui.Controls;
using ActionViewModel = PipManager.ViewModels.Pages.Action.ActionViewModel;

namespace PipManager.Views.Pages.Action;

public partial class ActionPage : INavigableView<ActionViewModel>
{
    public ActionViewModel ViewModel { get; }

    public ActionPage(ActionViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    private void TextBox_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
    {

    }
}