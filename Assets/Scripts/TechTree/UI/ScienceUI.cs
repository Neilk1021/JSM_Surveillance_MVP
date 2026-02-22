using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance;
using JSM.Surveillance.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Surveillance.TechTree
{
    public class ScienceUI : MonoBehaviour
    {
        [SerializeField] private ImageNameGroup groupPrefab;

        private readonly Dictionary<Resource, ImageNameGroup> _currentLayout = new();
        private RectTransform _root;
        
        private void OnEnable()
        {
            ResourceManager.OnResourcesChanged += ReloadUI;
            UnlockedManager.OnItemsUnlocked += ReloadUI;
            ReloadUI();
        }

        private void OnDisable()
        {
            ResourceManager.OnResourcesChanged -= ReloadUI;
            UnlockedManager.OnItemsUnlocked -= ReloadUI;
        }
        
        
        private void ReloadUI()
        {
            HashSet<Resource> scienceResources = ResourceManager.GetUnlockedScienceResources();
            scienceResources.ExceptWith(_currentLayout.Keys.ToHashSet());

            foreach (var v in scienceResources) {
                _currentLayout.TryAdd(v, Instantiate(groupPrefab, transform));
            }

            foreach (var kvp in _currentLayout)
            {
                kvp.Value.Load($"{ResourceManager.GetResource(kvp.Key)}", kvp.Key.Sprite);
            }

            _root ??= GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_root);
        }
    }
}
