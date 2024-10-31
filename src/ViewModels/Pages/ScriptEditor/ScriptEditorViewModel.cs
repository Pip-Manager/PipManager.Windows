using PipManager.Windows.Services.MonacoEditor;
using PipManager.Windows.Views.Pages.ScriptEditor;
using Serilog;
using Wpf.Ui.Abstractions.Controls;

namespace PipManager.Windows.ViewModels.Pages.ScriptEditor;

public partial class ScriptEditorViewModel(IMonacoEditorService monacoEditorService): ObservableObject, INavigationAware
{
    [ObservableProperty]
    private bool _monacoExists;
    
    private bool _isInitialized;

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