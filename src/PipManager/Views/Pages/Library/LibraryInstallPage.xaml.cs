using Wpf.Ui.Controls;
using LibraryInstallViewModel = PipManager.ViewModels.Pages.Library.LibraryInstallViewModel;

namespace PipManager.Views.Pages.Library;

public partial class LibraryInstallPage : INavigableView<LibraryInstallViewModel>
{
    public LibraryInstallViewModel ViewModel { get; }

    public LibraryInstallPage(LibraryInstallViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}