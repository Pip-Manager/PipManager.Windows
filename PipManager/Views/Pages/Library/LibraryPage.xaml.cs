using PipManager.ViewModels.Pages;
using Wpf.Ui.Controls;
using LibraryViewModel = PipManager.ViewModels.Pages.Library.LibraryViewModel;

namespace PipManager.Views.Pages.Library;

public partial class LibraryPage : INavigableView<LibraryViewModel>
{
    public LibraryViewModel ViewModel { get; }

    public LibraryPage(LibraryViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}