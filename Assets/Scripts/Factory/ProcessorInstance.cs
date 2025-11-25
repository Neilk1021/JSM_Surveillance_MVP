using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class ProcessorInstance : MonoBehaviour
    {
        [SerializeField] private ProcessorData data;
        [SerializeField] private Recipe selectedRecipe;
        
        private Dictionary<Resource, int> inputResources = new();
        private Dictionary<Resource, int> outputResources = new();


        private float progress = 0f;
        private bool isRunning = false;

        public Dictionary<Resource, int> Inputs => inputResources;
        public Dictionary<Resource, int> Outputs => outputResources;
        public float Progress => progress;
        public Recipe Recipe => selectedRecipe;
        public ProcessorData Data => data;
        public bool IsRunning => isRunning;

        /// <summary>
        /// called to set what recipe processor will run
        /// </summary>
        public void SetRecipe(Recipe recipe)
        {
            selectedRecipe = recipe;
            progress = 0f;
            isRunning = false;
        }

        /// <summary>
        /// Runs a single production cycle of the processor
        /// </summary>
        public void ProcessCycle(float deltaTime)
        {
            if (selectedRecipe == null) return;

            if (!isRunning && InputAmountSatisfied())
            {
                ContumeInputs();
                isRunning = true;
                progress = 0f;
            }
            if (isRunning)
            {
                progress += deltaTime * data.Speed;
                if (progress >= selectedRecipe.Time)
                {
                    ProduceOutputs();
                    isRunning = false;
                    progress = 0f;
                }
            }
        }

        private bool InputAmountSatisfied()
        {
            foreach (var r in selectedRecipe.InputVolume)
            {
                if (!inputResources.ContainsKey(r.resource) || inputResources[r.resource] < r.amount)
                    return false;
            }
            return true;
        }

        private void ContumeInputs()
        {
            foreach (var r in selectedRecipe.InputVolume)
            {
                inputResources[r.resource] -= r.amount;
            }
        }

        private void ProduceOutputs()
        {
            foreach (var r in selectedRecipe.OutputVolume)
            {
                if (!outputResources.ContainsKey(r.resource))
                {
                    outputResources[r.resource] = 0;
                }
                outputResources[r.resource] += r.amount;
            }
        }

        /// <summary>
        /// Adds an input resource and amount
        /// </summary>
        public void AddInput(Resource resource, int amount)
        {
            if (!inputResources.ContainsKey(resource))
            {
                inputResources[resource] = 0;
            }
            inputResources[resource] += amount;
        }
    }
}