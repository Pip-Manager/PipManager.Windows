using Wpf.Ui.Controls;
using Library_LibraryInstallViewModel = PipManager.Windows.ViewModels.Pages.Library.LibraryInstallViewModel;
using LibraryInstallViewModel = PipManager.Windows.ViewModels.Pages.Library.LibraryInstallViewModel;

namespace PipManager.Windows.Views.Pages.Library;

public partial class LibraryInstallPage : INavigableView<Library_LibraryInstallViewModel>
{
    public Library_LibraryInstallViewModel ViewModel { get; }

    public LibraryInstallPage(Library_LibraryInstallViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}