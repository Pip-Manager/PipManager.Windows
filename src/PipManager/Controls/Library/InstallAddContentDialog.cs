using PipManager.Languages;
using PipManager.Models.Pages;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using TextBox = System.Windows.Controls.TextBox;

namespace PipManager.Controls.Library;

public class InstallAddContentDialog(ContentPresenter contentPresenter)
{
    private readonly ContentDialog _contentDialog = new(contentPresenter)
    {
        PrimaryButtonText = Lang.ContentDialog_PrimaryButton_Action,
        CloseButtonText = Lang.ContentDialog_CloseButton_Cancel,
        Title = Lang.ContentDialog_Title_Warning,
        Content = Application.Current.TryFindResource("LibraryInstallAddContentDialogContent")
    };

    public async Task<string> ShowAsync()
    {
        await _contentDialog.ShowAsync();
        return ((_contentDialog.Content as Grid)!.Children[1] as TextBox)!.Text;
    }
}