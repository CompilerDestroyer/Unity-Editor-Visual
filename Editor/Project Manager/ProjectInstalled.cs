using CompilerDestroyer.Editor.EditorVisual;
using System.IO;
using UnityEditor;

public class ProjectInstalled
{
    [InitializeOnLoadMethod]
    private static void CreateInstalledTempFile()
    {
        if (!File.Exists(GlobalVariables.ProjectTempInstalledFilePath))
        {
            File.WriteAllText(GlobalVariables.ProjectTempInstalledFilePath, "Already Embedded Package!");
        }
    }
}
