using Wpf.Ui.Abstractions.Controls;
using LibraryInstallViewModel = PipManager.Windows.ViewModels.Pages.Library.LibraryInstallViewModel;

namespace PipManager.Windows.Views.Pages.Library;

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