using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JSM.Surveillance.UI
{
    public class ProcessorUI : MachineInfoUI
    {
        [Header("Prefabs")]
        [SerializeField] private ImageNameGroup imageNameGroup;
        
        [Header("References")]
        [SerializeField] Transform machineProducingParent;
        [SerializeField] Transform machineRequirementsParent;
        [SerializeField] private ChangeRecipeUI changeRecipeUI;
        [SerializeField] private Button changeRecipeButton;
        [SerializeField] private Button moveButton;
        [SerializeField] private Button sellButton;
        
        private ProcessorObject _machine;

        private ImageNameGroup _producingUI;
        private ImageNameGroup[] _requirementsUIArray;
        public override void Initialize(CellOccupier occupier, UIManager manager)
        {
            base.Initialize(occupier, manager);
            
            _machine = (ProcessorObject)occupier;
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

        public override void Sell()
        {
            _machine.Sell();
            Close();
        }

        public override void Move()
        {
            _machine.Move();
            Close();
        }
        public void ChangeRecipe()
        {
            changeRecipeUI.gameObject.SetActive(true);
            changeRecipeUI.Load(_machine);
        }

        private void ClearOldProducing()
        {
            if (_producingUI ==null) {
                return;
            }
            
            Destroy(_producingUI.gameObject);
            _producingUI = null;
        }

        private void BuildProducingSection(Recipe machineRecipe)
        {
            ClearOldProducing();

            if (machineRecipe == null) {
                _producingUI = Instantiate(imageNameGroup, machineProducingParent);
                _producingUI.Load($"Nothing.", null);
                return;
            }

            _producingUI = Instantiate(imageNameGroup, machineProducingParent);
            _producingUI.Load($"{machineRecipe.OutputVolume.amount} - {machineRecipe.OutputVolume.resource.ResourceName}", machineRecipe.OutputVolume.resource.Sprite);
        }
        
        
        private void ClearOldRequirements()
        {
            if (_requirementsUIArray == null) {
                return;
            }
            
            foreach (var go in _requirementsUIArray)
            {
                if (go != null)
                {
                    Destroy(go.gameObject); 
                }
            }

            _requirementsUIArray = null;
        }

        private void BuildRequirementsSection(Recipe machineRecipe)
        {
            ClearOldRequirements();
            if (machineRecipe == null) {
                return;
            }

            List<ImageNameGroup> imageNameGroups = new List<ImageNameGroup>();
            if (imageNameGroups == null) throw new ArgumentNullException(nameof(imageNameGroups));
            
            foreach (var vol in machineRecipe.InputVolumes)
            {
                var temp = Instantiate(imageNameGroup, machineRequirementsParent);
                temp.Load($"{vol.amount} - {vol.resource.ResourceName}", vol.resource.Sprite); 
                imageNameGroups.Add(temp);
            }

            _requirementsUIArray = imageNameGroups.ToArray();
        }

        
        public void Reload()
        {
            machineNameText.text = _machine.Data.ShopInfo.name;

            BuildRequirementsSection(_machine.Recipe);
            BuildProducingSection(_machine.Recipe);
            
            changeRecipeButton.enabled = FactoryGrid.Editable;
            moveButton.enabled = FactoryGrid.Editable;
            sellButton.enabled = FactoryGrid.Editable;
        }
    }


}