using System.Diagnostics;
using System.Drawing;
using System.IO;
using Microsoft.Web.WebView2.Wpf;
using PipManager.Core.Configuration.Models;
using PipManager.Windows.Views.Windows;

namespace PipManager.Windows.Services.MonacoEditor;

public class MonacoEditorService: IMonacoEditorService
{
    public WebView2? MonacoWebView { get; set; }
    public bool MonacoExists { get; set; } = true;
    private string CodeTempFilePath { get; init; } = Path.Combine(Path.GetTempPath(), "pipManager-temp-code.py");
    
    public async Task RunScript(EnvironmentModel environment, string code)
    {
        if (MonacoWebView is null)
        {
            return;
        }
        
        await File.WriteAllTextAsync(CodeTempFilePath, code);
        
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = environment.PythonPath,
                Arguments = CodeTempFilePath,
                UseShellExecute = true,
                CreateNoWindow = false
            }
        };
        
        process.Start();
        await process.WaitForExitAsync();
    }
    
    public void Initialize()
    {
        var monacoIndexPath = Path.Combine(System.Environment.CurrentDirectory, "Monaco/index.html");
        
        // Temp File
        if (File.Exists(CodeTempFilePath))
        {
            File.Delete(CodeTempFilePath);
        }
        
        if (!File.Exists(monacoIndexPath))
        {
            MonacoExists = false;
            return;
        }

        MonacoWebView = new WebView2();
        MonacoWebView.DefaultBackgroundColor = Color.Transparent;
        MonacoWebView.Source = new Uri(monacoIndexPath);
    }
}