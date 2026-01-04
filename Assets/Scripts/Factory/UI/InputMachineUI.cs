using System;
using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class InputMachineUI : MachineInfoUI
    {
        [SerializeField] private TextMeshProUGUI producingText;    
        private InputMachineObject _machineObject;
        public override void Initialize(CellOccupier occupier, UIManager manager)
        {
            base.Initialize(occupier, manager);
            
            _machineObject = (InputMachineObject)occupier;
            if (_machineObject == null) {
                throw new ArgumentException("Tried to initialize Machine UI with non machine instance.");
            }

            machineNameText.text = _machineObject.Source.SourceName;
            producingText.text = $"<b>Produces:</b>\n {_machineObject.Source.GetRawResourceRate()}x {_machineObject.Source.resource.ResourceCategory} / HR";
        }

    }
}