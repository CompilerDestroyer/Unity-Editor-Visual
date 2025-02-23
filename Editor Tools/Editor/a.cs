using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using System.Linq;

namespace CompilerDestroyer.Editor.FolderIcons
{
    static class EmbedPackageExample
    {
        static string targetPackage = "com.compilerdestroyer.editortools";
        static EmbedRequest Request;
        static ListRequest listRequest;

        [MenuItem("Window/Embed Package Example")]
        static void GetPackageName()
        {
            
        }

        static void LProgress()
        {
            if (listRequest.IsCompleted)
            {
                if (listRequest.Status == StatusCode.Success)
                {

                    if (listRequest.Result.Any(pkg => pkg.name == targetPackage))
                    {
                        Embed(targetPackage);
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

                EditorApplication.update -= LProgress;
            }
        }

        static void Embed(string inTarget)
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