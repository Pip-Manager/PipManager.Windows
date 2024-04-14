using PipManager.Languages;
using PipManager.Models.Pages;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace PipManager.Resources.Library;

public class DeletionWarningContentDialog
{
    private readonly ContentDialog _contentDialog;
    private List<LibraryListItem> LibraryList { get; set; }

    public DeletionWarningContentDialog(ContentPresenter? contentPresenter, List<LibraryListItem> libraryList)
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