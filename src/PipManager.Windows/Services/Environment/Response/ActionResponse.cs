using PipManager.Windows.Models;

namespace PipManager.Windows.Services.Environment.Response;

public class ActionResponse
{
    public bool Success { get; init; }
    public ExceptionType? Exception { get; set; }
    public string Message { get; init; } = string.Empty;
}