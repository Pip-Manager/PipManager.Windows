using PipManager.ViewModels.Pages.Tools;
using Wpf.Ui.Controls;

namespace PipManager.Views.Pages.Tools;

public partial class ToolsPage : INavigableView<ToolsViewModel>
{
    public ToolsViewModel ViewModel { get; }

    public ToolsPage(ToolsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}