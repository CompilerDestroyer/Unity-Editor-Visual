using UnityEngine;
using System;
using UnityEditor;
using System.IO;
using UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using UnityEditor.PackageManager.Requests;
using System.Collections.Generic;
using System.Linq;
using CompilerDestroyer.Editor.EditorVisual;

public class GitDependencyManager
{
    private static AddAndRemoveRequest packageInstallationRequest;
    private static string currentPackageName;


    [InitializeOnLoadMethod]
    private static void ApplyOnPackageRegistered()
    {
        if (!File.Exists(GlobalVariables.ProjectTempInstalledFilePath))
        {
            Events.registeredPackages += OnPackagesRegistered;
        }
    }

    private static void OnPackagesRegistered(PackageRegistrationEventArgs packageRegistrationInfo)
    {
        List<string> dependencies = new List<string>();

        foreach (PackageInfo packageInfo in packageRegistrationInfo.added)
        {
            string packageJsonPath = packageInfo.resolvedPath + Path.DirectorySeparatorChar + "package.json";
            if (File.Exists(packageJsonPath))
            {
                string jsonContent = File.ReadAllText(packageJsonPath);
                PackageJson packageData = JsonUtility.FromJson<PackageJson>(jsonContent);
                currentPackageName = packageData.name;

                if (packageData != null)
                {
                    if (packageData.gitDependencies != null)
                    {
                        AddGitDependencies(ref jsonContent, ref packageData, packageInfo, ref dependencies);
                    }
                }
            }
        }
        if (dependencies.Count == 0)
        {
            return;
        }

        dependencies = dependencies.Distinct().ToList();

        List<PackageInfo> installedPackages = PackageInfo.GetAllRegisteredPackages().ToList();
        dependencies.RemoveAll((dependency) => IsInCollection(dependency, installedPackages));

        InstallDependencies(dependencies);
    }

    private static void AddGitDependencies(ref string jsonContent, ref PackageJson packageData, PackageInfo packageInfo, ref List<string> gitDependencies)
    {
        if (packageData.gitDependencies.Length > 0 && packageData.gitDependencies != null)
        {
            for (int i = 0; i < packageData.gitDependencies.Length; i++)
            {
                string packageName = packageData.gitDependencies[i];

                gitDependencies.Add(packageName);
            }
        }
    }
    private static bool IsInCollection(string dependency, List<PackageInfo> collection)
    {
        if (collection == null)
        {
            return false;
        }
        for (int i = 0; i < collection.Count; i++)
        {
            PackageInfo package = collection[i];
            string repositoryUrl = package.packageId.Substring(package.packageId.IndexOf('@') + 1);
            if (repositoryUrl == dependency)
            {
                return true;
            }
        }

        return false;
    }

    private static void InstallDependencies(List<string> dependencies)
    {
        if (dependencies == null || dependencies.Count <= 0) { return; }

        string packageName = string.Join("\n", dependencies);
        packageName = packageName.Replace(".git", "");
        packageName = packageName.Replace("https://github.com/", "");

        if (!Application.isBatchMode &&
            !EditorUtility.DisplayDialog("Dependency Manager", "The following dependencies are required: \n" + packageName, "Install Dependencies", "Cancel"))
        {
            AddAndRemoveRequest removeRequest = Client.AddAndRemove(packagesToRemove: new string[] {currentPackageName});
            Debug.LogWarning("Cancelled installing dependencies. Removing Repository from packages.");
            return;
        }

        packageInstallationRequest = Client.AddAndRemove(dependencies.ToArray(), null);

        EditorUtility.DisplayProgressBar("Dependency Manager", "installing dependencies...", 0);
        EditorApplication.update += DisplayProgress;
    }
    private static void DisplayProgress()
    {
        if (packageInstallationRequest.IsCompleted)
        {
            EditorUtility.ClearProgressBar();
            EditorApplication.update -= DisplayProgress;
            Events.registeredPackages -= OnPackagesRegistered;
        }
    }
}

[Serializable]
public class PackageJson
{
    public string name;
    public string version;
    public string displayName;
    public string description;
    public string unity;
    public Author author;
    public string[] gitDependencies;
}

[Serializable]
public class Author
{
    public string name;
    public string email;
}
