﻿using PipManager.Models;
using PipManager.Models.AppConfigModels;

namespace PipManager.Services.Environment;

public interface IEnvironmentService
{
    public bool CheckEnvironmentExists(EnvironmentItem environmentItem);

    public (bool, string) CheckEnvironmentAvailable(EnvironmentItem environmentItem);

    public List<PackageItem>? GetLibraries();
    public string[] GetVersions(string packageName);
    public (bool, string) Update(string packageName);

    public (bool, string) Uninstall(string packageName);
}