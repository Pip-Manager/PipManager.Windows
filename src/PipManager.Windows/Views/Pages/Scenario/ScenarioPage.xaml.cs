using PipManager.Windows.ViewModels.Pages.Scenario;
using Wpf.Ui.Controls;

namespace PipManager.Windows.Views.Pages.Scenario;

public partial class ScenarioPage : INavigableView<ScenarioViewModel>
{
    public ScenarioViewModel ViewModel { get; }
    
    public ScenarioPage(ScenarioViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}