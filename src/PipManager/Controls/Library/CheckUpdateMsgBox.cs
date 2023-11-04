using PipManager.Languages;
using PipManager.Models.Pages;
using System.Windows.Controls;
using TextBlock = System.Windows.Controls.TextBlock;

namespace PipManager.Controls.Library;

public class CheckUpdateContentDialog
{
    private readonly MessageBox _messageBox;
    public List<LibraryCheckUpdateContentDialogContentListItem> LibraryList { get; set; }

    public CheckUpdateContentDialog(List<LibraryCheckUpdateContentDialogContentListItem> libraryList)
    {
        LibraryList = libraryList;
        _messageBox = new MessageBox
        {
            PrimaryButtonText = Lang.ContentDialog_PrimaryButton_Action,
            CloseButtonText = Lang.ContentDialog_CloseButton_Cancel,
            IsPrimaryButtonEnabled = false,
            MinWidth = 500,
            MinHeight = 100,
            MaxHeight = 500,
            Title = Lang.ContentDialog_Title_Notice,
            Content = Application.Current.TryFindResource("LibraryCheckUpdateContentDialogContent")
        };
        (((_messageBox.Content as Grid)!.Children[2] as ScrollViewer)!.Content as ItemsControl)!.ItemsSource = LibraryList;

        var needUpdate = LibraryList.Any(item => item.NeedUpdate);
        ((_messageBox.Content as Grid)!.Children[0] as TextBlock)!.Visibility =
            needUpdate ? Visibility.Visible : Visibility.Collapsed;
        (((_messageBox.Content as Grid)!.Children[2] as ScrollViewer)!.Content as ItemsControl)!.Visibility =
            needUpdate ? Visibility.Visible : Visibility.Collapsed;
        ((_messageBox.Content as Grid)!.Children[1] as TextBlock)!.Visibility =
            needUpdate ? Visibility.Collapsed : Visibility.Visible;
        _messageBox.IsPrimaryButtonEnabled = needUpdate;
    }

    public async Task<MessageBoxResult> ShowAsync()
    {
        return await _messageBox.ShowDialogAsync();
    }
}

public class LibraryCheckUpdateContentDialogContentListItem
{
    public LibraryCheckUpdateContentDialogContentListItem(LibraryListItem libraryListItem, string newVersion)
    {
        PackageName = libraryListItem.PackageName;
        PackageVersion = string.Format(Lang.Library_CheckUpdate_Current, libraryListItem.PackageVersion);
        NewVersion = string.Format(Lang.Library_CheckUpdate_Latest, newVersion);
        NeedUpdate = newVersion != libraryListItem.PackageVersion;
    }

    public string PackageName { get; set; }
    public string PackageVersion { get; set; }
    public string NewVersion { get; set; }
    public bool NeedUpdate { get; set; }
}