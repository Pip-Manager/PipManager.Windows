using PipManager.Languages;
using PipManager.Models.Pages;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using TextBlock = System.Windows.Controls.TextBlock;

namespace PipManager.Controls.Library;

public class CheckUpdateContentDialog
{
    private readonly ContentDialog _contentDialog;
    public List<LibraryCheckUpdateContentDialogContentListItem> LibraryList { get; set; }

    public CheckUpdateContentDialog(ContentPresenter contentPresenter, List<LibraryCheckUpdateContentDialogContentListItem> libraryList)
    {
        LibraryList = libraryList;
        _contentDialog = new ContentDialog(contentPresenter)
        {
            PrimaryButtonText = Lang.ContentDialog_PrimaryButton_Action,
            CloseButtonText = Lang.ContentDialog_CloseButton_Cancel,
            IsPrimaryButtonEnabled = false,
            Title = Lang.ContentDialog_Title_Notice,
            Content = Application.Current.TryFindResource("LibraryCheckUpdateContentDialogContent")
        };
        (((_contentDialog.Content as Grid)!.Children[2] as ScrollViewer)!.Content as ItemsControl)!.ItemsSource = LibraryList;

        var needUpdate = LibraryList.Any(item => item.NeedUpdate);
        ((_contentDialog.Content as Grid)!.Children[0] as TextBlock)!.Visibility =
            needUpdate ? Visibility.Visible : Visibility.Collapsed;
        (((_contentDialog.Content as Grid)!.Children[2] as ScrollViewer)!.Content as ItemsControl)!.Visibility =
            needUpdate ? Visibility.Visible : Visibility.Collapsed;
        ((_contentDialog.Content as Grid)!.Children[1] as TextBlock)!.Visibility =
            needUpdate ? Visibility.Collapsed : Visibility.Visible;
        _contentDialog.IsPrimaryButtonEnabled = needUpdate;
    }

    public async Task<ContentDialogResult> ShowAsync()
    {
        return await _contentDialog.ShowAsync();
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