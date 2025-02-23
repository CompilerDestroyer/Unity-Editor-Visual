using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class RemoveScriptingDefineSymbolsFromBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    private static readonly string[] DEFINES_TO_REMOVE = { "FOLDER", "DEBUG_SYMBOL", "EXAMPLE_DEFINE" };
    private string allDefineSymbols;

    // Called BEFORE the build starts
    public void OnPreprocessBuild(BuildReport report)
    {
        NamedBuildTarget buildTarget = NamedBuildTarget.FromBuildTargetGroup(report.summary.platformGroup);
        string defineSymbols = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
        allDefineSymbols = defineSymbols;

        Debug.Log($"Current define symbols: {defineSymbols}");

        // Remove each symbol in the array
        foreach (var define in DEFINES_TO_REMOVE)
        {
            if (defineSymbols.Contains(define))
            {
                defineSymbols = defineSymbols.Replace(define, "").Replace(";;", ";").Trim(';');
                Debug.Log($"Removed '{define}' from defines.");
            }
        }

        PlayerSettings.SetScriptingDefineSymbols(buildTarget, defineSymbols);
        Debug.Log($"Updated defines: {defineSymbols}");
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        NamedBuildTarget buildTarget = NamedBuildTarget.FromBuildTargetGroup(report.summary.platformGroup);

        EditorApplication.delayCall += () =>
        {
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, allDefineSymbols);
            Debug.Log($"Restored define symbols: {allDefineSymbols}");
        };
    }
}
