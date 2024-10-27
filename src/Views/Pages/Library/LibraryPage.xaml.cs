using Wpf.Ui.Abstractions.Controls;
using LibraryViewModel = PipManager.Windows.ViewModels.Pages.Library.LibraryViewModel;

namespace PipManager.Windows.Views.Pages.Library;

public partial class LibraryPage : INavigableView<LibraryViewModel>
{
    public LibraryViewModel ViewModel { get; }

    public LibraryPage(LibraryViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    private void ClearSelectionsButton_OnClick(object sender, RoutedEventArgs e)
    {
        LibraryList.SelectedItems.Clear();
    }
}