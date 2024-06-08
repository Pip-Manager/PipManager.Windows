using Wpf.Ui.Controls;
using Library_LibraryDetailViewModel = PipManager.Windows.ViewModels.Pages.Library.LibraryDetailViewModel;
using LibraryDetailViewModel = PipManager.Windows.ViewModels.Pages.Library.LibraryDetailViewModel;

namespace PipManager.Windows.Views.Pages.Library;

public partial class LibraryDetailPage : INavigableView<Library_LibraryDetailViewModel>
{
    public Library_LibraryDetailViewModel ViewModel { get; }

    public LibraryDetailPage(Library_LibraryDetailViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}