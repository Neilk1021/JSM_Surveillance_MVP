using System;
using JSM.Surveillance.Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSM.Surveillance.UI
{
    [RequireComponent(typeof(Source))]
    public class SourceHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Source _source;
        [SerializeField] private SourceHoverUI sourceHoverUIPrefab;
        
        private SourceHoverUI _sourceHoverUI;

        private void Awake()
        {
            _source = GetComponent<Source>();
        }

        private void SpawnHoverUI()
        {
            if(_sourceHoverUI != null) return;
            
            _sourceHoverUI = Instantiate(sourceHoverUIPrefab, _source.transform.position, Quaternion.identity);
            _sourceHoverUI.Load(_source);
        }

        private void DestroyHoverUI()
        {
            if (_sourceHoverUI != null) {
                Destroy(_sourceHoverUI.gameObject);
            }

            _sourceHoverUI = null;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!_source.IsPlaced) return;
            SpawnHoverUI();   
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DestroyHoverUI();
        }
    }
}