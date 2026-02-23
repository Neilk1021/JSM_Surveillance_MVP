using JSM.Surveillance;
using UnityEngine;
using UnityEngine.Serialization;

namespace Surveillance.TechTree
{
    [System.Serializable]
    public class Node
    {
        [FormerlySerializedAs("Name")] public string name;
        [FormerlySerializedAs("ID")] public int id;
        [FormerlySerializedAs("IsUnlocked")] public bool isUnlocked = false;
        [FormerlySerializedAs("Description")] public string description;
        [FormerlySerializedAs("Unlockables")] [SerializeReference] public Unlockable[] unlockables;
        [FormerlySerializedAs("ParentIDs")] public int[] parentIDs;
        public ResourceVolume[] requiredResources;


        public bool Purchase()
        {
            if (!ResourceManager.ConsumeResources(requiredResources)) return false;
            Unlock();
            return true;
        }

        public void Unlock()
        {
            isUnlocked = true;
            UnlockedManager.UnlockList(unlockables);
        }
    }
}