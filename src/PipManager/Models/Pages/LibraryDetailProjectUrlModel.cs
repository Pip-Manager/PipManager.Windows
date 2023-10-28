using Wpf.Ui.Controls;

namespace PipManager.Models.Pages;

public class LibraryDetailProjectUrlModel
{
    public required SymbolIcon Icon { get; set; }
    public required string UrlType { get; set; }
    public required string Url { get; set; }
}