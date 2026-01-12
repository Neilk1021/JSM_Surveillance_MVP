using System;
using UnityEngine;

namespace JSM.Surveillance.Visuals
{
    [RequireComponent(typeof(InputMachineObject))]
    public class InputMachineRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer debugSprite;
        private InputMachineObject _machineObject;
        private FactoryGrid _grid;
        
        

        private void Awake() {
            _machineObject = GetComponent<InputMachineObject>();
        }

        private void Start() {
            _grid = _machineObject.Grid;
        }

        
        public void Update() {
            if(FactoryGrid.Editable) return;
            if(_grid.FactoryGridSimulation == null) return;

            if(_grid.FactoryGridSimulation.MachineInstances.TryGetValue(_machineObject.GetGuid(), out var machineInstance)) {
                debugSprite.color = machineInstance.Output.amount != 0 ? Color.green : Color.red;
            }
        }
        
    }
}