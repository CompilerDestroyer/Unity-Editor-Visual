using UnityEditor.PackageManager;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using System.Linq;
using System.IO;

namespace CompilerDestroyer.Editor.EditorVisual
{
    public class ShowWin
    {
        static ListRequest listRequest;
        static EmbedRequest Request;
        static PackageInfo packageInfo;


        [MenuItem("Tools/Delete")]
        static void aha()
        {
            string currentScriptPath = "Packages/com.compilerdestroyer.editorvisual/Editor/Project Manager/EmbedProject";
            bool assetDeleted = AssetDatabase.DeleteAsset(currentScriptPath);

            AssetDatabase.Refresh();

            Debug.Log(assetDeleted);

        }


        [InitializeOnLoadMethod]
        private static void InitEmbeddingEditorVisualProject()
        {
            PackageSource packageInfo = PackageInfo.FindForPackageName(ProjectConstants.embeddedPackageName).source;

            if (packageInfo != PackageSource.Embedded && packageInfo != PackageSource.Local && packageInfo != PackageSource.LocalTarball)
            {
                Debug.Log("packa is embedding now!");
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
