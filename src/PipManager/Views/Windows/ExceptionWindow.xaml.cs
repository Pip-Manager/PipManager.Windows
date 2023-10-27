using System.Diagnostics;

namespace PipManager.Views.Windows;

/// <summary>
/// ExceptionWindow.xaml 的交互逻辑
/// </summary>
public partial class ExceptionWindow
{
    public ExceptionWindow()
    {
        InitializeComponent();
    }

    public void Initialize(Exception exception)
    {
        TypeTextBlock.Text = exception.GetType().ToString();
        MessageTextBlock.Text = exception.Message;
        StackTraceTextBox.Text = exception.StackTrace;
    }

    private void ReportButton_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start("explorer.exe", "https://github.com/Pip-Manager/PipManager/issues/new");
    }
}