using CompilerDestroyer.Editor.EditorVisual;
using System.IO;
using UnityEditor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

public class ProjectInstalled
{
    [InitializeOnLoadMethod]
    private static void CreateInstalledTempFile()
    {
        if (!File.Exists(GlobalVariables.ProjectTempInstalledFilePath))
        {
            PackageSource packageInfo = PackageInfo.FindForPackageName(ProjectConstants.embeddedPackageName).source;

            if (packageInfo == PackageSource.Embedded && packageInfo == PackageSource.Local && packageInfo == PackageSource.LocalTarball)
            {
                File.WriteAllText(GlobalVariables.ProjectTempInstalledFilePath, "Already Embedded Package!");
            }
        }
    }
}
