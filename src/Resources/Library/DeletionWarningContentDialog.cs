using System.Windows.Controls;
using PipManager.Windows.Languages;
using PipManager.Windows.Models.Pages;
using Wpf.Ui.Controls;

namespace PipManager.Windows.Resources.Library;

public class DeletionWarningContentDialog
{
    private readonly ContentDialog _contentDialog;
    private List<PackageListItem> LibraryList { get; set; }

    public DeletionWarningContentDialog(ContentPresenter? contentPresenter, List<PackageListItem> libraryList)
    {
        LibraryList = libraryList;
        _contentDialog = new ContentDialog(contentPresenter)
        {
            PrimaryButtonText = Lang.ContentDialog_PrimaryButton_Action,
            CloseButtonText = Lang.ContentDialog_CloseButton_Cancel,
            Title = Lang.ContentDialog_Title_Warning,
            Content = Application.Current.TryFindResource("LibraryDeletionWarningContentDialogContent")
        };
        (((_contentDialog.Content as Grid)!.Children[1] as ScrollViewer)!.Content as ItemsControl)!.ItemsSource = LibraryList;
    }

    public async Task<ContentDialogResult> ShowAsync()
    {
        return await _contentDialog.ShowAsync();
    }
}