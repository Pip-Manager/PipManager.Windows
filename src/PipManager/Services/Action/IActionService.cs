using PipManager.Models.Action;
using System.Collections.ObjectModel;

namespace PipManager.Services.Action;

public interface IActionService
{
    public ObservableCollection<ActionListItem> ActionList { get; set; }
    public ObservableCollection<ActionListItem> ExceptionList { get; set; }

    public void AddOperation(ActionListItem actionListItem);

    public void Runner();
}