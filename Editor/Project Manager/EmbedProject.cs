using UnityEditor.PackageManager;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using System.Linq;

namespace CompilerDestroyer.Editor.EditorVisual
{
    public class ShowWin
    {
        static ListRequest listRequest;
        static string targetPackage = "com.compilerdestroyer.editortools";
        static EmbedRequest Request;
        static PackageInfo packageInfo;



        //[InitializeOnLoadMethod]
        //private static void InitEmbeddingEditorVisualProject()
        //{

        //}

        [MenuItem("Tools/Check Packages")]
        static void GetWin()
        {
            PackageSource packageInfo = PackageInfo.FindForPackageName(ProjectConstants.embeddedPackageName).source;

            if (packageInfo != PackageSource.Embedded || packageInfo != PackageSource.Local || packageInfo != PackageSource.LocalTarball)
            {
                Debug.Log(packageInfo);
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

                    if (listRequest.Result.Any(pkg => pkg.name == targetPackage))
                    {
                        EmbedProject(targetPackage);
                    }
                    else
                    {
                        Debug.LogWarning("There is no: " + targetPackage + "Found!");
                    }
                }
                else
                {
                    Debug.Log(listRequest.Error.message);
                }

                EditorApplication.update -= ListProgress;

                Debug.Log(packageInfo);
            }
        }

        static void EmbedProject(string inTarget)
        {
            Debug.Log("Embed('" + inTarget + "') called");
            Request = Client.Embed(inTarget);
            EditorApplication.update += Progress;
        }

        static void Progress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                    Debug.Log("Embedded: " + Request.Result.packageId);
                else if (Request.Status >= StatusCode.Failure)
                    Debug.Log(Request.Error.message);

                EditorApplication.update -= Progress;
            }
        }
    }
}