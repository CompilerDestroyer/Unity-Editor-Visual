using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.IO;
using UnityEngine;
using CompilerDestroyer.Editor.UIElements;
using CompilerDestroyer.Editor.FolderIcons;

namespace CompilerDestroyer.Editor.ToolsManager
{
    public sealed class EditorToolsSettings : EditorWindow
    {
        private const string documentationName = "Documentation";
        private const string folderIconsName = "Folder Icons";
        private const string scriptDescriptionName = "Script Description";

        private static readonly Vector2 minWindowSize = new Vector2(310f, 200f);

        private List<TreeViewItemData<string>> projectSettingsList = new List<TreeViewItemData<string>>();
        private Dictionary<string, VisualElement> rootDict = new Dictionary<string, VisualElement>();

        [MenuItem("Tools/Compiler Destroyer/Editor Tools")]
        private static void ShowWindow()
        {
            EditorToolsSettings settingsWindow = GetWindow<EditorToolsSettings>();
            settingsWindow.titleContent.text = "Editor Tools Settings";
            settingsWindow.titleContent.image = EditorGUIUtility.FindTexture("SettingsIcon");
            settingsWindow.minSize = minWindowSize;
            
            //if (!SessionState.GetBool("IsCompilerButcherWindowInitiated", false))
            //{
            //    Rect mainEditorWindowRect = EditorGUIUtility.GetMainWindowPosition();
            //    Debug.Log("hee");
            //    // Calculate the center position for the editor window
            //    float centerX = mainEditorWindowRect.x + (mainEditorWindowRect.width - 734f) / 2;
            //    float centerY = mainEditorWindowRect.y + (mainEditorWindowRect.height - 438f) / 2;

            //    settingsWindow.position = new Rect(centerX, centerY, 734f, 438f);

            //    SessionState.SetBool("IsCompilerButcherWindowInitiated", true);
            //}
        }
        private void OnEnable()
        {
            Undo.undoRedoPerformed += FolderIconsSettings.RefreshIconSetListViewOnUndo;
        }
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= FolderIconsSettings.RefreshIconSetListViewOnUndo;
        }

        public void CreateGUI()
        {

            TreeViewItemData<string> folderIconsSetting = new TreeViewItemData<string>(0, folderIconsName);
            TreeViewItemData<string> scriptDescriptionSetting = new TreeViewItemData<string>(1, scriptDescriptionName);
            TreeViewItemData<string> documentationSetting = new TreeViewItemData<string>(2, documentationName);


            rootDict.Add(documentationName, null);
            rootDict.Add(folderIconsName, FolderIconsSettings.FolderIconsSettingsVisualElement());
            rootDict.Add(scriptDescriptionName, null);


            projectSettingsList.Add(documentationSetting);
            projectSettingsList.Add(folderIconsSetting);
            projectSettingsList.Add(scriptDescriptionSetting);

            SettingsPanel settingsWindow = new SettingsPanel(ref projectSettingsList, ref rootDict);

            rootVisualElement.Add(settingsWindow);
        }



        private void ApplyButton()
        {
            if (GUILayout.Button("Apply!"))
            {
                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
                AssetDatabase.Refresh();
            }
        }
        private void EnableOrDisableProjects(ref bool toggle, string toggleName, string findAssetFilter, ref string[] searchInFolders,
                                bool hasExtraSettings = false, Action extraEditorSettings = null)
        {
            string scriptContent;

            if (toggle)
            {
                string[] currentPath = AssetDatabase.FindAssets(findAssetFilter, searchInFolders);
                Debug.Log(currentPath.Length);

                for (int i = 0; i < currentPath.Length; i++)
                {
                    string editorScriptName = AssetDatabase.GUIDToAssetPath(currentPath[i]).Replace("\\", "/");
                    scriptContent = File.ReadAllText(editorScriptName);

                    string textToAddAtStart = "/*";
                    string textToAddAtEnd = "*/";


                    string newContent = textToAddAtStart + scriptContent + textToAddAtEnd;

                    File.WriteAllText(editorScriptName, newContent);
                }
            }
            else
            {
                string[] currentPath = AssetDatabase.FindAssets(findAssetFilter, searchInFolders);
                for (int i = 0; i < currentPath.Length; i++)
                {
                    string editorScriptName = AssetDatabase.GUIDToAssetPath(currentPath[i]).Replace("\\", "/");

                    scriptContent = File.ReadAllText(editorScriptName);


                    string newContent = scriptContent[2..^2];

                    File.WriteAllText(editorScriptName, newContent);
                }
            }
        }
    }
}
