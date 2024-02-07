using Wpf.Ui.Controls;
using SearchDetailViewModel = PipManager.ViewModels.Pages.Search.SearchDetailViewModel;

namespace PipManager.Views.Pages.Search;

public partial class SearchDetailPage : INavigableView<SearchDetailViewModel>
{
    public SearchDetailViewModel ViewModel { get; }

    public SearchDetailPage(SearchDetailViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}