using System;
using JSM.Surveillance.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JSM.Surveillance.Visuals
{
    [RequireComponent(typeof(InputMachineObject))]
    public class InputMachineRenderer : MonoBehaviour
    {
        [SerializeField] private ItemFillAmntUI itemFillUI;
        private InputMachineObject _machineObject;
        private FactoryGrid _grid;
        
        

        private void Awake() {
            _machineObject = GetComponent<InputMachineObject>();
        }

        private void Start() {
            _grid = _machineObject.Grid;
            itemFillUI.UpdateSlider(0, _machineObject.InventorySize);
        }

        
        public void Update() {
            if(FactoryGrid.Editable) return;
            if(_grid.FactoryGridSimulation == null) return;

            if (_grid.FactoryGridSimulation.MachineInstances
                .TryGetValue(_machineObject.GetGuid(), out var machineInstance))
            {
                itemFillUI.UpdateSlider(
                    machineInstance.Output.amount, 
                    _machineObject.InventorySize, 
                    machineInstance.Output.resource?.ResourceName 
                    );
            }
        }
        
    }
}