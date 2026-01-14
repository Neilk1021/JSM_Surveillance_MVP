
using System.Linq;
using JSM.Surveillance.UI;
using UnityEngine;


namespace JSM.Surveillance.Visuals
{
    [RequireComponent(typeof(ProcessorObject))]
    public class ProcessorRenderer : MonoBehaviour
    {
        [SerializeField] private ItemFillAmntUI [] itemFillUI;
        [SerializeField] private ItemFillAmntUI outputFillUI;
        
        private ProcessorObject _machineObject;
        private FactoryGrid _grid;

        private void Awake() {
            _machineObject = GetComponent<ProcessorObject>();
        }

        private void Start() {
            _grid = _machineObject.Grid;
            
            foreach (ItemFillAmntUI fillUI in itemFillUI)
            {
                fillUI.UpdateSlider(0, _machineObject.InventorySize);
            }
            
            outputFillUI.UpdateSlider(0, _machineObject.InventorySize);
        }

        
        public void Update() {
            if(FactoryGrid.Editable) return;
            if(_grid.FactoryGridSimulation == null) return;

            if (!_grid.FactoryGridSimulation.MachineInstances
                    .TryGetValue(_machineObject.GetGuid(), out var machineInstance)) return;

            if(machineInstance is not ProcessorInstance processorInstance) return;
            
            
            //TODO move this to some sort of event function
            var index = 0;
            foreach (var kvp in processorInstance.Inputs.Where(kvp => processorInstance.Recipe.RequiresInput(kvp.Key)))
            {
                if(index >= itemFillUI.Length) return;
                itemFillUI[index].UpdateSlider(
                    kvp.Value, 
                    _machineObject.InventorySize, 
                    kvp.Key?.ResourceName,
                    processorInstance.Recipe?.RequiredAmount(kvp.Key) > kvp.Value
                );
                
                ++index;
            }

            for (var i = index; i < itemFillUI.Length; i++) {
                itemFillUI[i].UpdateSlider(0, _machineObject.InventorySize);
            }
            
            
            outputFillUI.UpdateSlider(
                processorInstance.Output.amount, 
                _machineObject.InventorySize,
                processorInstance.Output.resource?.ResourceName
                );
        }

    }
}