namespace PipManager.Models.Package;

public class PackageVersion
{
    public string Epoch { get; set; } = "";
    public string Release { get; set; } = "";
    public string PreL { get; set; } = "";
    public string PreN { get; set; } = "";
    public string PostN1 { get; set; } = "";
    public string PostL { get; set; } = "";
    public string PostN2 { get; set; } = "";
    public string DevL { get; set; } = "";
    public string DevN { get; set; } = "";
    public string Local { get; set; } = "";
}