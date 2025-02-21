namespace CompilerButcher.Editor.DescriptionEditor
{
    using System;
    using UnityEngine;


    [Serializable]
    internal sealed class Description
    {
        [SerializeField] internal string monoBehaviourPath;
        [SerializeField] internal string type;
        [SerializeField] internal string description;
    }
}
