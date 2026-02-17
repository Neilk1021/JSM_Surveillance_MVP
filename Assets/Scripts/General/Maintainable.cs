using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace JSM.Surveillance
{
    
    [System.Serializable]
    public class ShopInfo 
    {
        public string name;
        [TextArea(minLines:5, maxLines:10)]
        public string desc; 
        public GameObject itemModelPrefab;
        public VideoClip videoClip;
    }
    
    [CreateAssetMenu(fileName = "Generic", menuName = "JSM/Surveillance/Maintainable/Generic")]

    public class Maintainable : ScriptableObject, ISerializationCallbackReceiver
    {
        public string AssetGuid { get; private set; }
        [SerializeField] protected int upfrontCost;
        [SerializeField] protected int dailyCost;

        public int DailyCost => dailyCost;
        public int UpfrontCost => upfrontCost;
        
        
        [SerializeField] private ShopInfo shopInfo;

        public ShopInfo ShopInfo => shopInfo;
        public void OnBeforeSerialize()
        {
            #if UNITY_EDITOR
                string path = AssetDatabase.GetAssetPath(this);
                AssetGuid = AssetDatabase.AssetPathToGUID(path);
            #endif

        }
        public void OnAfterDeserialize()
        {
        }
    }
}
