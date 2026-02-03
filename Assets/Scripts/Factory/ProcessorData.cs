using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewProcessor", menuName = "JSM/Surveillance/Maintainable/Processor")]
    public class ProcessorData : Maintainable, ISerializationCallbackReceiver
    {
        public string AssetGuid { get; private set; }
        [SerializeField] private RecipeBank recipeBank;
        [SerializeField] private float speed;

        public RecipeBank RecipeBank => recipeBank;
        public float Speed => speed;
        public void OnBeforeSerialize()
        {
            #if UNITY_EDITOR
                string path = AssetDatabase.GetAssetPath(this);
                AssetGuid = AssetDatabase.AssetPathToGUID(path);
            #endif
        }

        public void OnAfterDeserialize() { }
    }
}