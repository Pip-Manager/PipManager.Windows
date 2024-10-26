using Wpf.Ui.Abstractions.Controls;
using LibraryDetailViewModel = PipManager.Windows.ViewModels.Pages.Library.LibraryDetailViewModel;

namespace PipManager.Windows.Views.Pages.Library;

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