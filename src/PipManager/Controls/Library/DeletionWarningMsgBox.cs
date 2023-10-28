using PipManager.Languages;
using PipManager.Models.Pages;
using System.Windows.Controls;

namespace PipManager.Controls.Library;

public class DeletionWarningMsgBox
{
    private readonly MessageBox _messageBox;
    public List<LibraryListItem> LibraryList { get; set; }

    public DeletionWarningMsgBox(List<LibraryListItem> libraryList)
    {
        LibraryList = libraryList;
        _messageBox = new MessageBox
        {
            PrimaryButtonText = Lang.MsgBox_PrimaryButton_Action,
            CloseButtonText = Lang.MsgBox_CloseButton_Cancel,
            MinWidth = 500,
            MinHeight = 300,
            MaxHeight = 500,
            Title = Lang.MsgBox_Title_Warning,
            Content = Application.Current.TryFindResource("LibraryDeletionWarningMsgBoxContent")
        };
        (((_messageBox.Content as Grid)!.Children[1] as ScrollViewer)!.Content as ItemsControl)!.ItemsSource = LibraryList;
    }

    public async Task<MessageBoxResult> ShowAsync()
    {
        return await _messageBox.ShowDialogAsync();
    }
}