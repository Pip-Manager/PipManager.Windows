using PipManager.ViewModels.Pages;
using Wpf.Ui.Controls;
using SearchViewModel = PipManager.ViewModels.Pages.Search.SearchViewModel;

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