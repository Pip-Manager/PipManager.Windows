using System.Drawing;
using System.IO;
using Microsoft.Web.WebView2.Wpf;

namespace PipManager.Windows.Services.MonacoEditor;

public class MonacoEditorService: IMonacoEditorService
{
    public WebView2? MonacoWebView { get; set; }
    public bool MonacoExists { get; set; } = true;
    
    public void Initialize()
    {
        var monacoIndexPath = Path.Combine(System.Environment.CurrentDirectory, "Monaco/index.html");
        if (!Path.Exists(monacoIndexPath))
        {
            MonacoExists = false;
            return;
        }

        MonacoWebView = new WebView2();
        MonacoWebView.DefaultBackgroundColor = Color.Transparent;
        MonacoWebView.Source = new Uri(monacoIndexPath);
    }
}