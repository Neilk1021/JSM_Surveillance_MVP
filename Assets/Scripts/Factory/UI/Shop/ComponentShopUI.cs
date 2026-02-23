using System;
using System.Collections;
using System.Collections.Generic;
using Surveillance.TechTree;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSM.Surveillance.UI
{
    public class ComponentShopUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private static readonly int OpenShop = Animator.StringToHash("OpenShop");
        private static readonly int CloseShop = Animator.StringToHash("CloseShop");
        
        
        [SerializeField] private ComponentShopElementUI componentUIElementPrefab;
        [SerializeField] private ShopBank bank;
        [Tooltip("The transform that all the UI prefabs should be spawned under.")]
        [SerializeField] private Transform contentView;

        [SerializeField] private Animator animator; 
        
        private FactoryGrid _grid;
        private bool _opened = false;

        private List<Transform> _shopElements = new List<Transform>();
        
        private void Awake()
        {
            if (Camera.main != null) _cc = Camera.main.GetComponent<CameraController>();
            _grid = GetComponentInParent<FactoryGrid>();
        }

        private void OnEnable(){
            UnlockedManager.OnItemsUnlocked += ReloadShopItems;
            ReloadShopItems();
        }

        private void OnDisable() {
            UnlockedManager.OnItemsUnlocked -= ReloadShopItems;
        }

        private void ClearShopItems()
        {
            for (int i = _shopElements.Count - 1; i >= 0; i--)
            {
                Destroy(_shopElements[i].gameObject);
                _shopElements.RemoveAt(i);
            }
        }
        
        private void ReloadShopItems()
        {
            ClearShopItems();
            var filtered = UnlockedManager.FilterUnlockedMachines(bank.BuyableProcessors);
            foreach (var processor in filtered) 
            {
                var element = Instantiate(componentUIElementPrefab, contentView);
                element.Load(processor, this);
                _shopElements.Add(element.transform);
            }
        }

        private CameraController _cc;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _cc?.SetScrollActive(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _cc?.SetScrollActive(true);
        }


        public void SwitchShop()
        {
            _opened = !_opened;

            if (_opened) {
                animator.SetTrigger(OpenShop);
            }
            else {
                animator.SetTrigger(CloseShop);
            }
        }
        public void Buy(MachineObject processorPrefab)
        {
            if(!FactoryGrid.Editable) return;

            Maintainable data = null;
            switch (processorPrefab)
            {
                case ProcessorObject p0:
                    data = p0.Data;
                    break;
                case SplitterObject s0:
                    data = s0.Data;
                    break;
                case MergerObject m0:
                    data = m0.Data;
                    break;
            }
            
            if(data == null) return;
            
            if (!SurveillanceGameManager.instance.MoneyManager.ChangeMoneyBy(-data.UpfrontCost)) {
                return;
            }
            
            var processorObj = Instantiate(processorPrefab, _grid.transform);
            processorObj.StartPlacement();
        }
    }
}
