using System;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class ItemPreviewer : MonoBehaviour
    {
        public static ItemPreviewer Instance;
        private GameObject _currentPreview = null;
        [SerializeField] private Transform previewHolder;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public static void LoadPreview(GameObject newPrefab)
        {
            if (Instance == null)
            {
                Debug.LogError("No Item Previewer in scene!");
                return;
            }
            
            Instance.Init(newPrefab);
        }
        
        private void Init(GameObject newPreview)
        {
            if (_currentPreview != null)
            {
                Destroy(_currentPreview);
            }
            _currentPreview = Instantiate(newPreview, previewHolder);
        }
        
    }
}