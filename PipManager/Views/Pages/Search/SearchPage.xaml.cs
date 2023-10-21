using PipManager.ViewModels.Pages.Search;
using Wpf.Ui.Controls;
using ActionViewModel = PipManager.ViewModels.Pages.Search.SearchViewModel;

namespace PipManager.Views.Pages.Search;

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