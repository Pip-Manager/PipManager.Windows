using PipManager.Windows.Languages;
using Wpf.Ui;

namespace PipManager.Windows.Controls;

public static class ContentDialogCreateOptions
{
    public static SimpleContentDialogCreateOptions Error(string message, string primaryButtonText) => new()
    {
        Title = Lang.ContentDialog_Title_Error,
        Content = message,
        PrimaryButtonText = primaryButtonText,
        CloseButtonText = Lang.ContentDialog_CloseButton_Cancel
    };

    public static SimpleContentDialogCreateOptions Warning(string message, string primaryButtonText) => new()
    {
        Title = Lang.ContentDialog_Title_Warning,
        Content = message,
        PrimaryButtonText = primaryButtonText,
        CloseButtonText = Lang.ContentDialog_CloseButton_Cancel
    };

    public static SimpleContentDialogCreateOptions Notice(string message) => new()
    {
        Title = Lang.ContentDialog_Title_Notice,
        Content = message,
        PrimaryButtonText = Lang.ContentDialog_PrimaryButton_Action,
        CloseButtonText = Lang.ContentDialog_CloseButton_Cancel
    };
}