using PipManager.Windows.Controls;
using PipManager.Windows.Languages;
using Wpf.Ui;

namespace PipManager.Windows.Services.Toast;

public class ToastService(IThemeService themeService) : IToastService
{
    public void Info(string message)
    {
        Controls.Toast.Show(Lang.ContentDialog_Title_Notice, message, new ToastOptions { Time = 2000, Theme = themeService.GetTheme(), ToastType = ToastType.Info });
    }

    public void Warning(string message)
    {
        Controls.Toast.Show(Lang.ContentDialog_Title_Warning, message, new ToastOptions { Time = 2000, Theme = themeService.GetTheme(), ToastType = ToastType.Warning });
    }

    public void Error(string message)
    {
        Controls.Toast.Show(Lang.ContentDialog_Title_Error, message, new ToastOptions { Time = 2000, Theme = themeService.GetTheme(), ToastType = ToastType.Error });
    }

    public void Success(string message)
    {
        Controls.Toast.Show(Lang.ContentDialog_Title_Success, message, new ToastOptions { Time = 2000, Theme = themeService.GetTheme(), ToastType = ToastType.Success });
    }
}