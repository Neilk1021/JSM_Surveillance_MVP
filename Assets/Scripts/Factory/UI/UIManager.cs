using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private FactoryUI defaultUI;
        [SerializeField] private MachineInfoUI processorUI;

        private FactoryUI _currentUI = null;
        private CellOccupier _currentData = null;
        
        public void SwitchUI(CellOccupier occupier)
        {
            if (_currentData == occupier)
            {
                DestroyCurrentUI();
                return;
            }
            
            DestroyCurrentUI();
        }

        public void SwitchUI(ProcessorInstance processorInstance)
        {
            if (_currentData == processorInstance)
            {
                DestroyCurrentUI();
                return;
            }
            
            DestroyCurrentUI();

            _currentData = processorInstance;
            _currentUI = Instantiate(processorUI, processorInstance.transform.position + Vector3.right * 5, Quaternion.identity);
            _currentUI.Initialize(processorInstance);
        }
        
        
        private void DestroyCurrentUI()
        {
            if (_currentUI != null)
            {
                _currentData = null;
                Destroy(_currentUI.gameObject);
            }
        }
    }
}