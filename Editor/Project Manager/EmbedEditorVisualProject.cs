using UnityEditor.PackageManager;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using System.Linq;
using System.IO;

using Debug = UnityEngine.Debug;

namespace CompilerDestroyer.Editor.EditorVisual
{
    public class EmbedEditorVisualProject
    {
        static ListRequest listRequest;
        static EmbedRequest Request;

        [InitializeOnLoadMethod]
        private static void InitEmbeddingEditorVisualProject()
        {
            if (File.Exists(GlobalVariables.ProjectTempInstalledFilePath)) return;


            PackageSource packageInfo = PackageInfo.FindForPackageName(ProjectConstants.embeddedPackageName).source;

            if (packageInfo != PackageSource.Embedded && packageInfo != PackageSource.Local && packageInfo != PackageSource.LocalTarball)
            {
                Debug.Log(GlobalVariables.UnityEditorVisualPackageName + " is embedding now!");
                TrySearchEmbeddedPackage();
            }
            else
            {
                Debug.Log("Else worked");
            }
        }

        private static void TrySearchEmbeddedPackage()
        {
            listRequest = Client.List();
            EditorApplication.update += ListProgress;
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
                    Debug.Log(listRequest.Error.message);
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
