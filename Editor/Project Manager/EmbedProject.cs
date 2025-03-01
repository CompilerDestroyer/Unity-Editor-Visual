using UnityEditor.PackageManager;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using System.Linq;
using System.IO;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

namespace CompilerDestroyer.Editor.EditorVisual
{
    public class ShowWin
    {
        static ListRequest listRequest;
        static EmbedRequest Request;
        static PackageInfo packageInfo;
        private const string packagePath = "Packages/com.compilerbutcher.editorvisual";


        [MenuItem("Tools/Clear EditorPrefs")]
        public static void ClearAllEditorPrefs()
        {
            EditorPrefs.DeleteAll();
            Debug.Log("All EditorPrefs have been cleared.");
        }
        [MenuItem("Tools/Update Editor Visual")]
        public static void UpdateEditorVisual()
        {
            if (!AssetDatabase.IsValidFolder(packagePath))
            {
                UnityEngine.Debug.LogError($"Package path not found: {packagePath}");
                return;
            }

            // Run Git Pull
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "pull origin main",
                WorkingDirectory = packagePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                    UnityEngine.Debug.Log($"Git Output:\n{output}");
                if (!string.IsNullOrEmpty(error))
                    UnityEngine.Debug.LogError($"Git Error:\n{error}");
            }

            // Refresh Unity assets after update
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Package updated successfully!");
        }

        private static string unityEditorVisualInstalledEditorPref = "UnityEditorVisualInstalled";
        [InitializeOnLoadMethod]
        private static void InitEmbeddingEditorVisualProject()
        {
            if (!EditorPrefs.GetBool(unityEditorVisualInstalledEditorPref))
            {
                EditorPrefs.SetBool(unityEditorVisualInstalledEditorPref, true);
            }
            else
            {
                return;
            }

            PackageSource packageInfo = PackageInfo.FindForPackageName(ProjectConstants.embeddedPackageName).source;

            if (packageInfo != PackageSource.Embedded && packageInfo != PackageSource.Local && packageInfo != PackageSource.LocalTarball)
            {
                Debug.Log("package is embedding now!");
                TrySearchEmbeddedPackage();
            }
        }

        private static void TrySearchEmbeddedPackage()
        {
            if (packageInfo == null)
            {
                listRequest = Client.List();
                EditorApplication.update += ListProgress;
            }
        }
        static void ListProgress()
        {
            if (listRequest.IsCompleted)
            {
                if (listRequest.Status == StatusCode.Success)
                {

                    if (listRequest.Result.Any(pkg => pkg.name == GlobalVariables.UnityEditorVisualPackageName))
                    {
                        EmbedProject(GlobalVariables.UnityEditorVisualPackageName);
                    }
                    else
                    {
                        //Debug.LogWarning("There is no: " + targetPackage + "Found!");
                    }
                }
                else
                {
                    //Debug.Log(listRequest.Error.message);
                }

                EditorApplication.update -= ListProgress;
            }
        }

        static void EmbedProject(string inTarget)
        {
            Request = Client.Embed(inTarget);
            EditorApplication.update += EmbedProgress;
        }

        static void EmbedProgress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                {
                    string currentScriptPath = "Packages/com.compilerdestroyer.editorvisual/Editor/Project Manager/EmbedProject.cs";
                    bool assetDeleted = AssetDatabase.DeleteAsset(currentScriptPath);
                    
                    AssetDatabase.Refresh();
                    Debug.Log(assetDeleted);
                    Debug.Log("deleted");
                }
                else if (Request.Status >= StatusCode.Failure)
                {
                    
                }

                EditorApplication.update -= EmbedProgress;
            }
        }
    }
}
