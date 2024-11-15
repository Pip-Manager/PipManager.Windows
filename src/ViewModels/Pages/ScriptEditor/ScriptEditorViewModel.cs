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
    
    private const string CodeModel = "try:\n    {code}\nexcept Exception as e:\n    print(f\"[PipManager - ScriptEditor] An error occurred: {e}\")\nfinally:\n    input(\"[PipManager - ScriptEditor] Press Enter to exit...\")";
    
    [RelayCommand]
    private async Task RunScript()
    {
        var environment = Configuration.AppConfig.SelectedEnvironment;
        if (environment is null)
        {
            return;
        }
        var code = await monacoEditorService.MonacoWebView!.ExecuteScriptAsync("editor.getValue();");
        code = code == null ? "input(\"[PipManager - ScriptEditor] Press Enter to exit...\")" : CodeModel.Replace("{code}", code[1..^1].Replace("\\r\\n", "\n    "));
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