using PipManager.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace PipManager.Views.Pages;

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