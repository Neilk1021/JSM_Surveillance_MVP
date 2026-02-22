using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance;
using Surveillance.TechTree;
using UnityEngine;

namespace Surveillance.TechTree
{
    public class UnlockedManager : MonoBehaviour
    {
        private static UnlockedManager _instance;

        private readonly Dictionary<string, Unlockable> _unlockedElements = new Dictionary<string, Unlockable>();

        private static UnlockedManager Instance
        {
            get
            {
                if (_instance is not null) return _instance;
                
                _instance = FindObjectOfType<UnlockedManager>();
                if (_instance != null) return _instance;
                
                Debug.LogWarning("No Unlock Manager in scene, building one.");
                var r = new GameObject
                {
                    name = "Unlock Manager"
                };
                _instance = r.AddComponent<UnlockedManager>();
                return _instance;
            }
        }
        
        public static event Action OnItemsUnlocked;
            
        private void Awake()
        {
            if (_instance == null || _instance == this) {
                _instance = this;
                return;
            }
        
            Destroy(gameObject);
        }

        public static List<MachineObject> FilterUnlockedMachines(IReadOnlyList<MachineSObj> unfilteredList)
        {
            List<MachineObject> output = new List<MachineObject>();
            foreach (var machineObject in unfilteredList)
            {
                if (machineObject.UnlockByDefault || Instance._unlockedElements.ContainsKey(machineObject.UnlockId)) {
                    output.Add(machineObject.MachineObject);
                }
            }

            return output;
        }
        
        public static void UnlockList(IEnumerable<Unlockable> unlockables)
        {
            foreach (var unlockable in  unlockables)
            {
                Instance._unlockedElements.TryAdd(unlockable.UnlockId, unlockable);
            }
            OnItemsUnlocked?.Invoke();
        }

        public static bool IsUnlocked(Unlockable unlockable)
        {
            return unlockable.UnlockByDefault || _instance._unlockedElements.ContainsKey(unlockable.UnlockId);
        }
        
        public static void Unlock(Unlockable unlockable)
        {
            Instance._unlockedElements.TryAdd(unlockable.UnlockId, unlockable);
            OnItemsUnlocked?.Invoke();
        }
        
        //public List<Maintainable>
    }   
}

