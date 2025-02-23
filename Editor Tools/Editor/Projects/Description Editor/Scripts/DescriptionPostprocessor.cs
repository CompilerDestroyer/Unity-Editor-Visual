namespace CompilerDestroyer.Editor.DescriptionEditor
{
    using UnityEngine;
    using UnityEditor;

    internal class DescriptionPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (importedAssets.Length > 0)
            {
                for (int i = 0; i < importedAssets.Length; i++)
                {
                    string importedAsset = importedAssets[i];
                    if (importedAsset.EndsWith(".cs"))
                    {
                        MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(importedAsset);

                        if (monoScript != null)
                        {
                            System.Type scriptType = monoScript.GetClass();

                            if (scriptType != null && scriptType.IsSubclassOf(typeof(MonoBehaviour)))
                            {
                                DescriptionEditor.monoBehaviourDescriptionSO = AssetDatabase.LoadAssetAtPath<MonoBehaviourDescriptionSO>(ProjectConstants.descriptionSOPath);


                                if (!didDomainReload)
                                {
                                    Description description = DescriptionEditor.monoBehaviourDescriptionSO.descriptionList.Find(description => description.type == scriptType.ToString());
                                    DescriptionEditor.monoBehaviourDescriptionSO.descriptionList.Remove(description);
                                }
                                else
                                {
                                    if (!DescriptionEditor.monoBehaviourDescriptionSO.descriptionList.Exists(description => description.type == scriptType.ToString()))
                                    {
                                        Description newDescription = new Description();
                                        newDescription.monoBehaviourPath = importedAsset;
                                        newDescription.type = scriptType.ToString();
                                        newDescription.description = "Double Click To Edit!";

                                        DescriptionEditor.monoBehaviourDescriptionSO.descriptionList.Add(newDescription);
                                    }
                                }
                            }
                        }
                    }

                }
            }


            if (deletedAssets.Length > 0)
            {
                for (int i = 0; i < deletedAssets.Length; i++)
                {
                    string deletedAsset = deletedAssets[i];
                    if (deletedAsset.EndsWith(".cs"))
                    {
                        Description description = DescriptionEditor.monoBehaviourDescriptionSO.descriptionList.Find(description => description.monoBehaviourPath == deletedAsset);
                        if (description != null)
                        {
                            DescriptionEditor.monoBehaviourDescriptionSO.descriptionList.Remove(description);
                        }
                    }
                }
            }

            if (movedFromAssetPaths.Length > 0)
            {
                for (int i = 0; i < movedFromAssetPaths.Length; i++)
                {
                    string currentPath = movedFromAssetPaths[i];
                    string targetPath = movedAssets[i];

                    if (currentPath.EndsWith(".cs"))
                    {
                        DescriptionEditor.monoBehaviourDescriptionSO = AssetDatabase.LoadAssetAtPath<MonoBehaviourDescriptionSO>(ProjectConstants.descriptionSOPath);

                        Description description = DescriptionEditor.monoBehaviourDescriptionSO.descriptionList.Find(description => description.monoBehaviourPath == currentPath);

                        if (description != null)
                        {
                            description.monoBehaviourPath = targetPath;
                            DescriptionEditor.SaveMonoBehaviourDescriptionAsset();
                        }
                    }
                }
            }
        }
    }
}
