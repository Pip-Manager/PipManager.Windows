using Wpf.Ui.Controls;
using Library_LibraryViewModel = PipManager.Windows.ViewModels.Pages.Library.LibraryViewModel;
using LibraryViewModel = PipManager.Windows.ViewModels.Pages.Library.LibraryViewModel;

namespace PipManager.Windows.Views.Pages.Library;

public partial class LibraryPage : INavigableView<Library_LibraryViewModel>
{
    public Library_LibraryViewModel ViewModel { get; }

    public LibraryPage(Library_LibraryViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}