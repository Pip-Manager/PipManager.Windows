using System.Windows.Controls;
using PipManager.Windows.Languages;
using Wpf.Ui.Controls;
using TextBox = System.Windows.Controls.TextBox;

namespace PipManager.Windows.Resources.Library;

public class InstallAddContentDialog(ContentPresenter? contentPresenter)
{
    private readonly ContentDialog _contentDialog = new(contentPresenter)
    {
        PrimaryButtonText = Lang.ContentDialog_PrimaryButton_Action,
        CloseButtonText = Lang.ContentDialog_CloseButton_Cancel,
        Title = Lang.ContentDialog_Title_Notice,
        DialogHeight = 200,
        Content = Application.Current.TryFindResource("LibraryInstallAddContentDialogContent"),
        IsSecondaryButtonEnabled = false,
        CloseButtonAppearance = ControlAppearance.Secondary
    };

    public async Task<string> ShowAsync()
    {
        var result = await _contentDialog.ShowAsync();
        return result == ContentDialogResult.None ? "" : ((_contentDialog.Content as Grid)!.Children[1] as TextBox)!.Text.Trim();
    }
}