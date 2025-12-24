using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.UI;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class ProcessorInstance : Draggable 
    {
        [SerializeField] private ProcessorData data;
        [SerializeField] private Recipe selectedRecipe;
        
        private readonly Dictionary<Resource, int> _inputResources = new();
        private readonly Dictionary<Resource, int> _outputResources = new();

        private float _progress = 0f;
        private bool _isRunning = false;

        public Dictionary<Resource, int> Inputs => _inputResources;
        public Dictionary<Resource, int> Outputs => _outputResources;
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

        public override void Place(List<Vector2Int> newPositions, Vector2 worldPos, FactoryGrid grid)
        {
            base.Place(newPositions, worldPos, grid);
            
            Vector2Int root = GetRootPosition();
            foreach (var port in GetComponentsInChildren<ProcessorPort>())
            {
                grid.RegisterPort(port.SubcellPosition + root, port);
            }
        }
        

        private bool InputAmountSatisfied()
        {
            foreach (var r in selectedRecipe.InputVolume)
            {
                if (!_inputResources.ContainsKey(r.resource) || _inputResources[r.resource] < r.amount)
                    return false;
            }
            return true;
        }

        private void ConsumeInputs()
        {
            foreach (var r in selectedRecipe.InputVolume)
            {
                _inputResources[r.resource] -= r.amount;
            }
        }

        private void ProduceOutputs()
        {
            foreach (var r in selectedRecipe.OutputVolume)
            {
                _outputResources.TryAdd(r.resource, 0);
                _outputResources[r.resource] += r.amount;
            }
        }

        /// <summary>
        /// Adds an input resource and amount
        /// </summary>
        public void AddInput(Resource resource, int amount)
        {
            _inputResources.TryAdd(resource, 0);
            _inputResources[resource] += amount;
        }

    }
}