using PipManager.Languages;

namespace PipManager.Controls;

public static class MsgBox
{
    public static async Task Error(string message)
    {
        var messageBox = new Wpf.Ui.Controls.MessageBox
        {
            Title = Lang.MsgBox_Title_Error,
            Content = message,
            MinWidth = 300,
            IsPrimaryButtonEnabled = false,
            CloseButtonText = Lang.MessageDialog_CloseButton_Cancel
        };
        await messageBox.ShowDialogAsync();
    }
}