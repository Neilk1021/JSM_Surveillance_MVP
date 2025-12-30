using System;
using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class ProcessorUI : MachineInfoUI
    {
        [SerializeField] TextMeshProUGUI machineProducingText;
        [SerializeField] private TextMeshProUGUI machineRequirementsText;
        
        [SerializeField] private ChangeRecipeUI changeRecipeUI;
        
        private ProcessorInstance _machine;
        public override void Initialize(CellOccupier occupier, UIManager manager)
        {
            base.Initialize(occupier, manager);
            
            _machine = (ProcessorInstance)occupier;
            if (_machine == null) {
                throw new ArgumentException("Tried to initialize Machine UI with non machine instance.");
            }
            
            Reload();
        }

        private string BuildProducingString(Recipe machineRecipe)
        {
            string output = "<b>Producing:</b>\n";

            if (machineRecipe == null)
            {
                output += $"Nothing.\nSelect a recipe";
                return output;
            }
            
            output += $"{machineRecipe.OutputVolume.amount}x - {machineRecipe.OutputVolume.resource.ResourceName}\n";

            return output;
        }

        private string BuildRequirementsString(Recipe machineRecipe)
        {
            string output = "<b>Requires:</b>\n";

            if (machineRecipe == null)
            {
                output += $"Nothing.";
                return output;
            }
            
            foreach (var volume in machineRecipe.InputVolumes)
            {
                output += $"{volume.amount}x - {volume.resource.ResourceName}\n";
            }

            return output;
        }

        public void Sell()
        {
            _machine.Sell();
            Close();
        }

        public void Move()
        {
            _machine.Move();
            Close();
        }
        public void ChangeRecipe()
        {
            changeRecipeUI.gameObject.SetActive(true);
            changeRecipeUI.Load(_machine);
        }
        
        public void Reload()
        {
            machineNameText.text = _machine.Data.ShopInfo.name;
            machineProducingText.text = BuildProducingString(_machine.Recipe);
            machineRequirementsText.text = BuildRequirementsString(_machine.Recipe);

        }
    }
}