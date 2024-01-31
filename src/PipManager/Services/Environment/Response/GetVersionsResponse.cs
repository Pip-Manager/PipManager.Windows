namespace PipManager.Services.Environment.Response;

public class GetVersionsResponse
{
    public int Status { get; set; } // 0: Success 1: Not found 2: Invalid Package Name
    public string[]? Versions { get; set; }
}