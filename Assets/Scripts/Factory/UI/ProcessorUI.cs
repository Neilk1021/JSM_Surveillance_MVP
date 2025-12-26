using System;
using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class ProcessorUI : MachineInfoUI
    {
        [SerializeField] TextMeshProUGUI machineProducingText;
        [SerializeField] private TextMeshProUGUI machineRequirementsText;
        
        private ProcessorInstance _machine;
        public override void Initialize(CellOccupier occupier, UIManager manager)
        {
            base.Initialize(occupier, manager);
            
            _machine = (ProcessorInstance)occupier;
            if (_machine == null) {
                throw new ArgumentException("Tried to initialize Machine UI with non machine instance.");
            }

            machineNameText.text = _machine.Data.ShopInfo.name;

            machineProducingText.text = BuildProducingString(_machine.Recipe);
            machineRequirementsText.text = BuildRequirementsString(_machine.Recipe);
        }

        private string BuildProducingString(Recipe machineRecipe)
        {
            string output = "<b>Producing:</b>\n";

            foreach (var volume in machineRecipe.OutputVolume)
            {
                output += $"{volume.amount}x - {volume.resource.ResourceName}\n";
            }

            return output;
        }

        private string BuildRequirementsString(Recipe machineRecipe)
        {
            string output = "<b>Requires:</b>\n";

            foreach (var volume in machineRecipe.InputVolume)
            {
                output += $"{volume.amount}x - {volume.resource.ResourceName}\n";
            }

            return output;
        }
    }
}