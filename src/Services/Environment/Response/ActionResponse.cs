using PipManager.Windows.Models;

namespace PipManager.Windows.Services.Environment.Response;

public class ActionResponse
{
    public bool Success { get; set; }
    public ExceptionType? Exception { get; set; }
    public string Message { get; set; } = string.Empty;
}