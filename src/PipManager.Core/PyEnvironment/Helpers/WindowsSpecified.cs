using System.Diagnostics;
using System.Runtime.Versioning;
using PipManager.Core.Configuration.Models;

namespace PipManager.Core.PyEnvironment.Helpers;


[SupportedOSPlatform("Windows")]
public static class WindowsSpecified
{
    public static EnvironmentModel? GetEnvironment(string pythonPath)
    {
        var pythonVersion = FileVersionInfo.GetVersionInfo(pythonPath).FileVersion!;
        var pipDirectory = GetPackageDirectory(Directory.GetParent(pythonPath)!.FullName);

        if (pipDirectory is null)
        {
            return null;
        }
        
        pipDirectory = Path.Combine(pipDirectory, "pip");
        var pipVersion = Common.GetPipVersionInInitFile().Match(File.ReadAllText(Path.Combine(pipDirectory, "__init__.py"))).Groups[1].Value;
        return new EnvironmentModel { Identifier = "", PipVersion = pipVersion, PythonPath = pythonPath, PythonVersion = pythonVersion};
    }
    
    private static string? GetPackageDirectory(string pythonDirectory)
    {
        var sitePackageDirectory = Path.Combine(pythonDirectory, @"Lib\site-packages");
        return !Directory.Exists(sitePackageDirectory) ? null : sitePackageDirectory;
    }

    public static EnvironmentModel? GetEnvironmentByCommand(string command, string arguments)
    {
         var proc = new Process
         {
             StartInfo = new ProcessStartInfo
             {
                 FileName = command,
                 Arguments = arguments,
                 UseShellExecute = false,
                 RedirectStandardOutput = true,
                 CreateNoWindow = true,
                 WindowStyle = ProcessWindowStyle.Hidden
             }
         };
         try
         {
             proc.Start();
         }
         catch
         {
             return null;
         }

         var pipVersion = "";
         var pythonVersion = "";
         var pipDir = "";
         while (!proc.StandardOutput.EndOfStream)
         {
             var output = proc.StandardOutput.ReadLine();
             if (string.IsNullOrWhiteSpace(output)) continue;
             var sections = output.Split(' ');
             var pipDirStart = false;
             for (var i = 0; i < sections.Length; i++)
             {
                 if (sections[i] == "from")
                 {
                     pipVersion = sections[i - 1];
                     pipDirStart = true;
                 }
                 else if (sections[i] == "(python")
                 {
                     pythonVersion = sections[i + 1].Replace(")", "");
                     break;
                 }
                 else if (pipDirStart)
                 {
                     pipDir += sections[i] + ' ';
                 }
             }
         }
         pipVersion = pipVersion.Trim();
         var pythonPath = FindPythonPathByPipDir(pipDir.Trim());
         pythonVersion = pythonVersion.Trim();
         proc.Close();
         return pipDir.Length > 0 ? new EnvironmentModel {
             Identifier = "",
             PipVersion = pipVersion,
             PythonPath = pythonPath,
             PythonVersion = pythonVersion
         } : null;
    }

    private static string FindPythonPathByPipDir(string pipDir)
     {
         // Need more information
         var pipExePath = Path.Combine(new DirectoryInfo(pipDir).Parent!.Parent!.Parent!.FullName,
             "python.exe");
         var pipExePathAttempt1 = Path.Combine(new DirectoryInfo(pipDir).Parent!.Parent!.FullName,
             "python.exe");
         if (!File.Exists(pipExePath))
         {
             pipExePath = pipExePathAttempt1;
         }

         return pipExePath;
     }
}