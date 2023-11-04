using PipManager.Languages;
using PipManager.Models.Pages;
using System.Windows.Controls;

namespace PipManager.Controls.Library;

public class DeletionWarningContentDialog
{
    private readonly MessageBox _messageBox;
    public List<LibraryListItem> LibraryList { get; set; }

    public DeletionWarningContentDialog(List<LibraryListItem> libraryList)
    {
        LibraryList = libraryList;
        _messageBox = new MessageBox
        {
            PrimaryButtonText = Lang.ContentDialog_PrimaryButton_Action,
            CloseButtonText = Lang.ContentDialog_CloseButton_Cancel,
            MinWidth = 600,
            MinHeight = 300,
            MaxHeight = 500,
            Title = Lang.ContentDialog_Title_Warning,
            Content = Application.Current.TryFindResource("LibraryDeletionWarningContentDialogContent")
        };
        (((_messageBox.Content as Grid)!.Children[1] as ScrollViewer)!.Content as ItemsControl)!.ItemsSource = LibraryList;
    }

    public async Task<MessageBoxResult> ShowAsync()
    {
        return await _messageBox.ShowDialogAsync();
    }
}