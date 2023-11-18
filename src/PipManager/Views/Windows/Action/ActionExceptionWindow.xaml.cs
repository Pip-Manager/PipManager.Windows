using PipManager.Helpers;
using PipManager.Languages;
using PipManager.Models.Action;

namespace PipManager.Views.Windows.Action;

/// <summary>
/// ActionExceptionWindow.xaml 的交互逻辑
/// </summary>
public partial class ActionExceptionWindow
{
    public ActionExceptionWindow()
    {
        InitializeComponent();
    }

    public void Initialize(ActionListItem? action)
    {
        SpeculationTextBox.Text = action.Analyze();
        OriginalExceptionTextBox.Text = Lang.ActionExceptionWindow_OriginalException + "\n" + action.ConsoleError;
    }
}