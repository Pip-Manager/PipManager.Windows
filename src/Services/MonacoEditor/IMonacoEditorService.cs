using Microsoft.Web.WebView2.Wpf;
using Wpf.Ui.Appearance;

namespace PipManager.Windows.Services.MonacoEditor;

public interface IMonacoEditorService
{
    public WebView2? MonacoWebView { get; set; }
    public bool MonacoExists { get; set; }
    public void Initialize();
}