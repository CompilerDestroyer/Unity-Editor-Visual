namespace CompilerButcher.Editor.DescriptionEditor
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(MonoBehaviour), true)]
    internal sealed class DescriptionEditor : Editor
    {
        private Description currentDescription;

        private bool isEditing = false;
        internal static MonoBehaviourDescriptionSO monoBehaviourDescriptionSO;

        private static readonly string recordObjectName = "Change MonoBehaviour Description";

        [MenuItem("CONTEXT/MonoBehaviour/Show\\Hide Description")]
        private static void ShowHideDescription()
        {
            Undo.RecordObject(monoBehaviourDescriptionSO, recordObjectName);
            monoBehaviourDescriptionSO.descriptionEditorEnabled = !monoBehaviourDescriptionSO.descriptionEditorEnabled;
            EditorUtility.SetDirty(monoBehaviourDescriptionSO);

            Selection.activeTransform = null;
        }

        private static void Initialize()
        {
            if (monoBehaviourDescriptionSO == null)
            {
                FindOrCreateAsset();
            }

            if (!monoBehaviourDescriptionSO.descriptionEditorEnabled) return;

            if (monoBehaviourDescriptionSO.butcherSkin == null)
            {
                monoBehaviourDescriptionSO.butcherSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(ProjectConstants.butcherSkinAssetPath);
            }

            SaveMonoBehaviourDescriptionAsset();

        }

        private void OnEnable()
        {
            Initialize();
            if (!monoBehaviourDescriptionSO.descriptionEditorEnabled) return;
            currentDescription = monoBehaviourDescriptionSO.descriptionList.Find((description) => description.type == target.GetType().ToString());
        }
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement rootVisualElement = new VisualElement();
            if (!monoBehaviourDescriptionSO.descriptionEditorEnabled)
            {
                return base.CreateInspectorGUI();
            }
            else
            {
                //AddScriptPartToTheInspector(ref rootVisualElement);
                HandleDoubleClickLabel(ref rootVisualElement);

                SerializedProperty property = serializedObject.GetIterator();
                if (property.NextVisible(true))
                {
                    do
                    {
                        // Skip the "m_Script" property
                        if (property.name == "m_Script")
                            continue;

                        PropertyField propertyField = new PropertyField(property);

                        propertyField.label = property.displayName;

                        rootVisualElement.Add(propertyField);

                    } while (property.NextVisible(false));
                }

                rootVisualElement.Bind(serializedObject);

                return rootVisualElement;
            }
        }

        //public override void OnInspectorGUI()
        //{
        //    serializedObject.Update();

        //    if (!monoBehaviourDescriptionSO.descriptionEditorEnabled)
        //    {
        //        DrawDefaultInspector();
        //        return;
        //    }

        //    serializedObject.ApplyModifiedProperties();
        //}

        private void HandleDoubleClickLabel()
        {
            Event currentEvent = Event.current;

            if (isEditing)
            {
                Undo.RecordObject(monoBehaviourDescriptionSO, recordObjectName);

                currentDescription.description = EditorGUILayout.TextArea(currentDescription.description, monoBehaviourDescriptionSO.butcherSkin.textArea);

                if (currentEvent.keyCode == KeyCode.Return)
                {
                    int selectedDescriptionIndex = monoBehaviourDescriptionSO.descriptionList.FindIndex(description => description == currentDescription);
                    monoBehaviourDescriptionSO.descriptionList[selectedDescriptionIndex].description = currentDescription.description;

                    EditorUtility.SetDirty(monoBehaviourDescriptionSO);

                    isEditing = false;
                }
                if (currentEvent.type == EventType.MouseDown)
                {
                    Rect textArea = GUILayoutUtility.GetLastRect();

                    if (currentEvent.type == EventType.MouseDown && !textArea.Contains(currentEvent.mousePosition))
                    {
                        isEditing = false;
                    }
                }
            }
            else
            {
                if (currentDescription != null)
                {
                    GUILayout.Box(currentDescription.description, monoBehaviourDescriptionSO.butcherSkin.box, GUILayout.ExpandWidth(true));
                    GUILayout.Height(50);

                    if (currentEvent.type == EventType.MouseDown && currentEvent.clickCount == 2)
                    {
                        Rect labelRect = GUILayoutUtility.GetLastRect();
                        if (labelRect.Contains(currentEvent.mousePosition))
                        {
                            isEditing = true;
                            currentDescription.type = target.GetType().ToString();
                            currentEvent.Use();
                        }
                    }
                }
            }
        }

        private void HandleDoubleClickLabel(ref VisualElement root)
        {
            Box descriptionBox = new Box();
            //Color col = new Color(Color.ba)
            descriptionBox.style.backgroundColor = Color.grey;
            Label labi = new Label("Shit");
            labi.style.unityTextAlign = TextAnchor.MiddleCenter;
            labi.style.unityFontStyleAndWeight = FontStyle.Bold;
            labi.style.fontSize = 15;


            descriptionBox.Add(labi);
            root.Add(descriptionBox);
        }

        internal static void SaveMonoBehaviourDescriptionAsset()
        {
            EditorUtility.SetDirty(monoBehaviourDescriptionSO);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private static void FindOrCreateAsset()
        {
            if (!AssetDatabase.AssetPathExists(ProjectConstants.descriptionSOPath))
            {
                Debug.LogWarning($"{(ProjectConstants.descriptionSOPath)} does not exists! Creating it...");
                monoBehaviourDescriptionSO = ScriptableObject.CreateInstance<MonoBehaviourDescriptionSO>();
                AssetDatabase.CreateAsset(monoBehaviourDescriptionSO, ProjectConstants.descriptionSOPath);
                SaveMonoBehaviourDescriptionAsset();
            }
            else
            {
                monoBehaviourDescriptionSO = AssetDatabase.LoadAssetAtPath<MonoBehaviourDescriptionSO>(ProjectConstants.descriptionSOPath);
            }

            if (monoBehaviourDescriptionSO.butcherSkin == null)
            {
                monoBehaviourDescriptionSO.butcherSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(ProjectConstants.butcherSkinAssetPath);
            }

            SaveMonoBehaviourDescriptionAsset();
        }

        // If you want "Script: placeholdermonobehaviour" part of the inspector you can add this top of OnInspectorGUI
        private void AddScriptPartToTheInspector()
        {
            SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
            GUI.enabled = false;
            EditorGUILayout.PropertyField(scriptProperty);
            GUI.enabled = true;
        }

        private void AddScriptPartToTheInspector(ref VisualElement root)
        {
            // Find the "m_Script" property
            SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");

            // Create a PropertyField for the "m_Script" property
            PropertyField scriptField = new PropertyField(scriptProperty);

            // Disable the field to prevent editing
            scriptField.SetEnabled(false);

            // Add the PropertyField to the root VisualElement
            root.Add(scriptField);
        }
    }
}