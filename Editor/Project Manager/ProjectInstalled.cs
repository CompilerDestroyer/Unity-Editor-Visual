using CompilerDestroyer.Editor.EditorVisual;
using System.IO;
using UnityEditor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using UnityEditor.PackageManager;
using System.Linq;
using CompilerDestroyer.Editor;

[InitializeOnLoad]
public class ProjectInstalled
{
    static ProjectInstalled()
    {
        CreateInstalledTempFile();

        Events.registeringPackages += DeleteInstalledTempFile;
    }

    private static void CreateInstalledTempFile()
    {
        if (!File.Exists(GlobalVariables.ProjectTempInstalledFilePath))
        {
            PackageSource packageInfo = PackageInfo.FindForPackageName(ProjectConstants.embeddedPackageName).source;

            if (packageInfo == PackageSource.Embedded || packageInfo == PackageSource.Local || packageInfo == PackageSource.LocalTarball)
            {
                File.WriteAllText(GlobalVariables.ProjectTempInstalledFilePath, "Already Embedded Package!");
            }
        }
    }


    private static void DeleteInstalledTempFile(PackageRegistrationEventArgs args)
    {
        PackageInfo packageInfo = args.removed.First((x) => x.name == GlobalVariables.UnityEditorVisualPackageName);

        if (packageInfo != null)
        {
            File.Delete(GlobalVariables.ProjectTempInstalledFilePath);
            Events.registeringPackages -= DeleteInstalledTempFile;
        }

        //if (packageInfo.source != PackageSource.Embedded && packageInfo.source != PackageSource.Local && packageInfo.source != PackageSource.LocalTarball)
        //{

        //}
    }
}