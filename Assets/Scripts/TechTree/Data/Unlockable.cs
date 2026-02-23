#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Surveillance.TechTree
{
    [System.Serializable]
    public abstract class Unlockable : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private bool unlockByDefault = false;

        private string _unlockId;  
        
        public bool UnlockByDefault => unlockByDefault;
        public string UnlockId => _unlockId;
        
        public virtual void OnBeforeSerialize()
        {
            #if UNITY_EDITOR
                string path = AssetDatabase.GetAssetPath(this);
                _unlockId = AssetDatabase.AssetPathToGUID(path);
            #endif
        }
        public virtual void OnAfterDeserialize() {
        }
    }
}