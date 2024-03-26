using PipManager.Languages;
using PipManager.Models.Pages;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using TextBlock = System.Windows.Controls.TextBlock;

namespace PipManager.Resources.Library;

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

public class LibraryCheckUpdateContentDialogContentListItem(LibraryListItem libraryListItem, string newVersion)
{
    public string PackageName { get; set; } = libraryListItem.PackageName;
    public string PackageVersion { get; set; } = string.Format(Lang.Library_CheckUpdate_Current, libraryListItem.PackageVersion);
    public string NewVersion { get; set; } = string.Format(Lang.Library_CheckUpdate_Latest, newVersion);
    public bool NeedUpdate { get; set; } = newVersion != libraryListItem.PackageVersion;
}