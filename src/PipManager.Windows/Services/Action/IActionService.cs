using Meziantou.Framework.WPF.Collections;
using System.Collections.ObjectModel;
using PipManager.Windows.Models.Action;

namespace PipManager.Windows.Services.Action;

public interface IActionService
{
    public ConcurrentObservableCollection<ActionListItem> ActionList { get; set; }
    public ObservableCollection<ActionListItem> ExceptionList { get; set; }

    public void AddOperation(ActionListItem actionListItem);

    public bool TryCancelOperation(string operationId);

    public void Runner();
}