namespace CompilerDestroyer.Editor.DescriptionEditor
{
    using System.Collections.Generic;
    using UnityEngine;


    internal sealed class MonoBehaviourDescriptionSO : ScriptableObject
    {
        [SerializeField] internal bool descriptionEditorEnabled = true;
        [SerializeField] internal List<Description> descriptionList = new List<Description>();
        [SerializeField] internal GUISkin butcherSkin;
    }
}

