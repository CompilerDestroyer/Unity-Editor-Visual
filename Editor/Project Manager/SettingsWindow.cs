using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.IO;
using UnityEngine;
using CompilerDestroyer.Editor.UIElements;

namespace CompilerDestroyer.Editor.EditorVisual
{
    public sealed class EditorVisualSettingsWindow : EditorWindow
    {
        private const string documentationName = "Documentation";
        private const string folderIconsName = "Folder Icons";

        private static readonly Vector2 minWindowSize = new Vector2(310f, 200f);

        private List<TreeViewItemData<string>> projectSettingsList = new List<TreeViewItemData<string>>();
        private Dictionary<string, VisualElement> rootDict = new Dictionary<string, VisualElement>();

        [MenuItem("Tools/Compiler Destroyer/Editor Visual")]
        private static void ShowWindow()
        {
            EditorVisualSettingsWindow settingsWindow = GetWindow<EditorVisualSettingsWindow>();
            settingsWindow.titleContent.text = "Editor Visual Settings";
            settingsWindow.titleContent.image = EditorGUIUtility.FindTexture("SettingsIcon");
            settingsWindow.minSize = minWindowSize;
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
            TreeViewItemData<string> documentationSetting = new TreeViewItemData<string>(2, documentationName);


            rootDict.Add(documentationName, null);
            rootDict.Add(folderIconsName, FolderIconsSettings.FolderIconsSettingsVisualElement());


            projectSettingsList.Add(documentationSetting);
            projectSettingsList.Add(folderIconsSetting);

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