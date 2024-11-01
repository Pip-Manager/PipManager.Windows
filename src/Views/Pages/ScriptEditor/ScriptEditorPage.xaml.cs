using PipManager.Windows.Services.MonacoEditor;
using PipManager.Windows.ViewModels.Pages.ScriptEditor;
using Wpf.Ui.Abstractions.Controls;

namespace PipManager.Windows.Views.Pages.ScriptEditor;

public partial class ScriptEditorPage : INavigableView<ScriptEditorViewModel>
{
    public ScriptEditorViewModel ViewModel { get; }

    public ScriptEditorPage(ScriptEditorViewModel viewModel, IMonacoEditorService monacoEditorService)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        if (monacoEditorService.MonacoExists)
        {
            MonacoContainer.Children.Add(monacoEditorService.MonacoWebView!);
        }
    }
}