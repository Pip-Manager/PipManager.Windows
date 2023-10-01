namespace PipManager.Models.EnvironmentModels;

public class PipItemModel
{
    public PipItemModel(string description, string pipVersion, string pythonVersion, string pipDir)
    {
        Description = description;
        PipVersion = pipVersion;
        PythonVersion = pythonVersion;
        PipDir = pipDir;
    }

    public string Description { get; set; }
    public string PipVersion { get; set; }
    public string PythonVersion { get; set;}
    public string PipDir { get; set; }
}