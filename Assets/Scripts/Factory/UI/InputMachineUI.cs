using System;
using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class InputMachineUI : MachineInfoUI
    {
        [SerializeField] private TextMeshProUGUI producingText;    
        private InputMachine _machine;
        public override void Initialize(CellOccupier occupier, UIManager manager)
        {
            base.Initialize(occupier, manager);
            
            _machine = (InputMachine)occupier;
            if (_machine == null) {
                throw new ArgumentException("Tried to initialize Machine UI with non machine instance.");
            }

            machineNameText.text = _machine.Source.SourceName;
            producingText.text = $"<b>Produces:</b>\nDEBUG, need to figure out how to calc first";
        }

    }
}