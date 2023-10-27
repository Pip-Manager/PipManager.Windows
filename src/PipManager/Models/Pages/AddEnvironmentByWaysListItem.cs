using Wpf.Ui.Controls;

namespace PipManager.Models.Pages;

public class AddEnvironmentByWaysListItem
{
    public AddEnvironmentByWaysListItem(SymbolIcon icon, string way)
    {
        SymbolIcon = icon;
        Way = way;
    }

    public SymbolIcon SymbolIcon { get; set; }
    public string Way { get; set; }
}