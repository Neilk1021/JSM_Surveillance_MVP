using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.UI;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class ProcessorInstance : MachineInstance 
    {
        [Header("Processor Info")]
        [SerializeField] private ProcessorData data;

        [SerializeField] private RecipeBank availableRecipes;
        [SerializeField] private Recipe selectedRecipe;
        private float _progress = 0f;
        private bool _processing = false;
        private bool _running = false; 

        public Recipe[] AvailableRecipes => availableRecipes.recipes;
        
        public float Progress => _progress;
        public Recipe Recipe => selectedRecipe;
        public ProcessorData Data => data;
        public bool Processing => _processing;
        public bool IsRunning => _running; 


        protected override void OnMouseDown()
        {
            if (Placed) {
                Grid.UIManager.SwitchUI(this);
            }

            base.OnMouseDown();
        }

        /// <summary>
        /// called to set what recipe processor will run
        /// </summary>
        public virtual void SetRecipe(Recipe recipe)
        {
            selectedRecipe = recipe;
            _progress = 0f;
            _processing = false;
            OutputResource.resource = recipe.OutputVolume.resource;
            OutputResource.amount = 0;
            MachineStateUpdated();
        }

        private void Update()
        {
            if (_running)
            {
                ProcessCycle(Time.deltaTime);
            }
        }

        /// <summary>
        /// Runs a single production cycle of the processor
        /// </summary>
        public void ProcessCycle(float deltaTime)
        {
            if (selectedRecipe == null) return;

            if (!_processing && InputAmountSatisfied())
            {
                ConsumeInputs();
                _processing = true;
                _progress = 0f;
            }
            if (_processing)
            {
                _progress += deltaTime * data.Speed;

                if (!(_progress >= selectedRecipe.Time)) {
                    return;
                }
                
                ProduceOutputs();
                _processing = false;
                _progress = 0f;
                MachineStateUpdated();
            }
        }

        private bool InputAmountSatisfied()
        {
            foreach (var r in selectedRecipe.InputVolumes)
            {
                if (!InputResources.ContainsKey(r.resource) || InputResources[r.resource] < r.amount)
                    return false;
            }
            return true;
        }

        private void ConsumeInputs()
        {
            foreach (var r in selectedRecipe.InputVolumes)
            {
                InputResources[r.resource] -= r.amount;
            }
        }

        private void ProduceOutputs()
        {
            if (OutputResource.resource != selectedRecipe.OutputVolume.resource)
            {
                OutputResource.resource = selectedRecipe.OutputVolume.resource;
                OutputResource.amount = 0;
            }
            
            OutputResource.amount += selectedRecipe.OutputVolume.amount;
            MachineStateUpdated();
        }



        public override void Sell()
        {
            SurveillanceGameManager.instance.MoneyManager.ChangeMoneyBy(data.UpfrontCost);
            base.Sell();
        }


        public void StartSimulation()
        {
            _running = true;
        }

        public void StopSimulation()
        {
            _running = false;
        }
        
    }
}