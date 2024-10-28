using System.ComponentModel;
using System.Diagnostics;

namespace PipManager.Windows.Views.Windows;

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
        if (exception.StackTrace != null)
        {
            StackTraceTextBox.Text = exception.StackTrace;
        }
    }

    private void ReportButton_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start("explorer.exe", "https://github.com/Pip-Manager/PipManager.Windows/issues/new");
    }

    private void ExceptionWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Environment.Exit(0);
    }
}