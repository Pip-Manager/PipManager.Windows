using PipManager.Core.Configuration;
using PipManager.Windows.Services.MonacoEditor;
using Serilog;
using Wpf.Ui.Abstractions.Controls;

namespace PipManager.Windows.ViewModels.Pages.ScriptEditor;

public partial class ScriptEditorViewModel(IMonacoEditorService monacoEditorService): ObservableObject, INavigationAware
{
    [ObservableProperty]
    private bool _monacoExists;
    
    private bool _isInitialized;
    
    [RelayCommand]
    private async Task RunScript()
    {
        var environment = Configuration.AppConfig.SelectedEnvironment;
        if (environment is null)
        {
            return;
        }
        var code = await monacoEditorService.MonacoWebView!.ExecuteScriptAsync("editor.getValue();");
        if (code == null)
        {
            code = "input('\\n[PipManager - ScriptEditor] Press Enter to close...')";
        }
        else
        {
            code = code[1..^1] + "\ninput('\\n[PipManager - ScriptEditor] Press Enter to close...')";
        }
        await monacoEditorService.RunScript(environment, code);
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        MonacoExists = monacoEditorService.MonacoExists;
        Log.Information("[Script Editor] Initialized");
    }

    public Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}