using PipManager.Windows.ViewModels.Pages.Tools;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace PipManager.Windows.Views.Pages.Tools;

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