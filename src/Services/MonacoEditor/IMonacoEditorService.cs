using Microsoft.Web.WebView2.Wpf;
using PipManager.Core.Configuration.Models;

namespace PipManager.Windows.Services.MonacoEditor;

public interface IMonacoEditorService
{
    public WebView2? MonacoWebView { get; set; }
    public bool MonacoExists { get; set; }

    public Task RunScript(EnvironmentModel environment, string code);
    public void Initialize();
}