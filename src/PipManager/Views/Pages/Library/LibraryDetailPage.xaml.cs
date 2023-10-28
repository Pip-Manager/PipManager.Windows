using Wpf.Ui.Controls;
using LibraryDetailViewModel = PipManager.ViewModels.Pages.Library.LibraryDetailViewModel;

namespace PipManager.Views.Pages.Library;

public partial class LibraryDetailPage : INavigableView<LibraryDetailViewModel>
{
    public LibraryDetailViewModel ViewModel { get; }

    public LibraryDetailPage(LibraryDetailViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}