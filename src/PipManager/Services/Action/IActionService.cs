using Meziantou.Framework.WPF.Collections;
using PipManager.Models.Action;
using System.Collections.ObjectModel;

namespace PipManager.Services.Action;

public interface IActionService
{
    public ConcurrentObservableCollection<ActionListItem> ActionList { get; set; }
    public ObservableCollection<ActionListItem> ExceptionList { get; set; }

    public void AddOperation(ActionListItem actionListItem);

    public bool TryCancelOperation(string operationId);

    public void Runner();
}