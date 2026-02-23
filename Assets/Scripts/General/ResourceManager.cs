using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Surveillance.TechTree;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    public class ResourceManager : MonoBehaviour
    {
        private const string ResourceBankLookup = "Assets/ScriptableObjects/ResourceBank.asset"; 
        private static ResourceManager _instance;
        private static ResourceManager Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<ResourceManager>();
                if (_instance != null) return _instance;
                
                Debug.LogWarning("No ResourceManager in scene, building one.");
                var r = new GameObject
                {
                    name = "ResourceManager"
                };
                _instance = r.AddComponent<ResourceManager>();
                return _instance;
            }
        }

        [SerializeField] private ResourceBank resourceBank;
        [SerializeField] private ResourceVolume[] defaultResources;
        
        private readonly Dictionary<Resource, int> _resources = new Dictionary<Resource, int>();

        public static event Action OnResourcesChanged;
        private static List<Resource> _scienceCache; 
        
        private void Awake()
        {
            if (_instance == null || _instance == this) {
                _instance = this;
                StartCoroutine(EnsureInitialized());
                return;
            }
            
            Destroy(gameObject);
        }

        private void Start()
        {
            foreach (var vol in defaultResources) {
                AddResource(vol.resource, vol.amount);
            }
        }

        public static void AddResource(Resource resource, int amount)
        {
            if (Instance._resources.TryAdd(resource, amount))
            {
                OnResourcesChanged?.Invoke();
                return;
            }
            
            Instance._resources[resource] += amount;
            OnResourcesChanged?.Invoke();
        }
        
        public static int GetResource(Resource resource)
        {
            return Instance._resources.GetValueOrDefault(resource, 0);
        }

        public static bool ConsumeResource(Resource resource, int amount)
        {
            if (GetResource(resource) < amount) {
                return false;
            }

            Instance._resources.TryAdd(resource, 0);
            Instance._resources[resource] -= amount;
            OnResourcesChanged?.Invoke();
            return true;
        }

        
        public static bool ConsumeResource(ResourceVolume resourceVolume) {
            return ConsumeResource(resourceVolume.resource, resourceVolume.amount);
        }
        
        public static bool ConsumeResources(IList<ResourceVolume> resourceVolumes)
        {
            bool valid = !resourceVolumes.Any(x => GetResource(x.resource) < x.amount);
            if (!valid) return false;

            foreach (var vol in resourceVolumes)
            {
                Instance._resources.TryAdd(vol.resource, 0);
                Instance._resources[vol.resource] -= vol.amount;
            }
            OnResourcesChanged?.Invoke();
            return true;
        }

        private static IEnumerator EnsureInitialized()
        {
            if (Instance.resourceBank != null) yield break;
            var op = Addressables.LoadAssetAsync<ResourceBank>(ResourceBankLookup); 
            
            yield return op;
            if (op.Status == AsyncOperationStatus.Succeeded) {
                Instance.resourceBank = op.Result;
            }
        }

        private static List<Resource> GetScienceResources()
        {
            return _scienceCache ??= Instance.resourceBank.resources
                .Where(x => x.IsScience)
                .ToList();
        }

        public static HashSet<Resource> GetUnlockedScienceResources()
        {
            return GetScienceResources().Where(x => UnlockedManager.IsUnlocked(x)).ToHashSet();
        }
    }
}