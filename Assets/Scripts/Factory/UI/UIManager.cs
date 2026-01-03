using System;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI prefabs")]
        [SerializeField] private FactoryUI defaultUI;
        [SerializeField] private InputMachineUI inputUI;
        [SerializeField] private OutputMachineUI outputUI;
        [SerializeField] private MachineInfoUI processorUI;

        [Header("Components Reference")] [SerializeField]
        private Canvas canvas;

        
        private FactoryUI _currentUI = null;
        private CellOccupier _currentData = null;
        public Camera WorldCamera => canvas.worldCamera;

        public bool IsUIOpen()
        {
            return _currentUI != null;
        }
        
        private void Awake() {
            canvas.worldCamera = GetComponentInParent<Camera>();
        }

        public void SwitchUI(InputMachineObject inputMachineObject)
        {
            if (IsCurrentMachine(inputMachineObject)) return;
            SetUI(inputUI, inputMachineObject);
        }
        
        public void SwitchUI(ProcessorObject processorObject)
        {
            if (IsCurrentMachine(processorObject)) return;
            SetUI(processorUI, processorObject);
        }

        public void SwitchUI(ExternalInputObject inputMachineObject)
        {
            throw new NotImplementedException();
        }

        private void SetUI(FactoryUI ui, CellOccupier value)
        {
            _currentData = value;
            _currentUI = Instantiate(ui, value.transform.position + Vector3.right * 5, Quaternion.identity);
            _currentUI.Initialize(_currentData, this);   
        }
        
        private bool IsCurrentMachine(CellOccupier occupier) {
            if (_currentData == occupier && !_currentUI.MouseInside)
            {
                Close();
                return true;
            }

            Close();
            return false;
        }

  

        public void Close()
        {
            if (_currentUI == null) return;
            
            Destroy(_currentUI.gameObject);
            _currentData = null;
        }

    }
}