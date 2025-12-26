using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.UI;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class ProcessorInstance : MachineInstance 
    {
        [SerializeField] private ProcessorData data;
        [SerializeField] private Recipe selectedRecipe;

        private float _progress = 0f;
        private bool _isRunning = false;


        public float Progress => _progress;
        public Recipe Recipe => selectedRecipe;
        public ProcessorData Data => data;
        public bool IsRunning => _isRunning;


        protected override void OnMouseDown()
        {
            if (Placed) {
                FactoryGrid.ActiveGrid.UIManager.SwitchUI(this);
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
            _isRunning = false;
        }

        /// <summary>
        /// Runs a single production cycle of the processor
        /// </summary>
        public void ProcessCycle(float deltaTime)
        {
            if (selectedRecipe == null) return;

            if (!_isRunning && InputAmountSatisfied())
            {
                ConsumeInputs();
                _isRunning = true;
                _progress = 0f;
            }
            if (_isRunning)
            {
                _progress += deltaTime * data.Speed;
                if (_progress >= selectedRecipe.Time)
                {
                    ProduceOutputs();
                    _isRunning = false;
                    _progress = 0f;
                }
            }
        }

        private bool InputAmountSatisfied()
        {
            foreach (var r in selectedRecipe.InputVolume)
            {
                if (!InputResources.ContainsKey(r.resource) || InputResources[r.resource] < r.amount)
                    return false;
            }
            return true;
        }

        private void ConsumeInputs()
        {
            foreach (var r in selectedRecipe.InputVolume)
            {
                InputResources[r.resource] -= r.amount;
            }
        }

        private void ProduceOutputs()
        {
            foreach (var r in selectedRecipe.OutputVolume)
            {
                OutputResources.TryAdd(r.resource, 0);
                OutputResources[r.resource] += r.amount;
            }
        }

        /// <summary>
        /// Adds an input resource and amount
        /// </summary>
        public void AddInput(Resource resource, int amount)
        {
            InputResources.TryAdd(resource, 0);
            InputResources[resource] += amount;
        }

    }
}