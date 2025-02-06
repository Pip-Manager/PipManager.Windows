using System.Windows.Input;
using PipManager.Core.Wrappers.PackageSearchIndexWrapper;
using PipManager.Windows.ViewModels.Pages.Search;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

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

    private void SearchListItem_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListViewItem item)
        {
            ViewModel.SearchItemDoubleClickCommand.Execute(item.DataContext as string);
        }
    }
}