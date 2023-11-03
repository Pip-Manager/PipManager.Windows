namespace PipManager.Models.Package;

public class PackageVersion
{
    public PackageVersion()
    {
    }

    public PackageVersion(string epoch, string release, string preL, string preN, string postN1, string postL, string postN2, string devL, string devN, string local)
    {
        Epoch = epoch;
        Release = release;
        PreL = preL;
        PreN = preN;
        PostN1 = postN1;
        PostL = postL;
        PostN2 = postN2;
        DevL = devL;
        DevN = devN;
        Local = local;
    }

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