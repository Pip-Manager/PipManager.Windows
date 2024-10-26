using PipManager.Windows.ViewModels.Pages.Search;
using Wpf.Ui.Abstractions.Controls;

namespace PipManager.Windows.Views.Pages.Search;

public partial class SearchPage : INavigableView<SearchViewModel>
{
    public SearchViewModel ViewModel { get; }

    public SearchPage(SearchViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}