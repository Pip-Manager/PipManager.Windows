using System.Runtime.InteropServices;
using PipManager.Core.Configuration.Models;
using PipManager.Core.PyEnvironment.Helpers;

namespace PipManager.Core.PyEnvironment;

public static class Detector
{
    public static List<EnvironmentModel> ByEnvironmentVariable()
    {
        var environmentItems = new List<EnvironmentModel>();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var value = Environment.GetEnvironmentVariable("Path")!.Split(';');
            foreach (var item in value)
            {
                if (!File.Exists(Path.Combine(item, "python.exe")))
                    continue;
                var environmentItem = WindowsSpecified.GetEnvironment(Path.Combine(item, "python.exe"));
                if (environmentItem == null)
                {
                    continue;
                }
                environmentItems.Add(environmentItem);
            }
        }
        else
        {
            throw new PlatformNotSupportedException("OS is not supported.");
        }
        return environmentItems;
    }
    
    public static EnvironmentModel? ByPythonPath(string pythonPath, string identifier = "")
    {
        if (!File.Exists(pythonPath))
        {
            throw new FileNotFoundException("Python path is not found.");
        }
        
        EnvironmentModel? environment;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            environment = WindowsSpecified.GetEnvironment(pythonPath);
            
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            environment = UnixSpecified.GetEnvironment(pythonPath);
        }
        else
        {
            throw new PlatformNotSupportedException("OS is not supported.");
        }
        
        if(environment != null)
        {
            environment.Identifier = identifier;
        }
        return environment;
    }
}