using System;
using JSM.Surveillance.Saving;
using UnityEngine;

namespace JSM.Surveillance
{
    [System.Serializable]
    public partial class ProcessorInstance : MachineInstance
    {
        private ProcessorData _data;
        private int ticks = 0;
        private bool _processing = false;

        public ProcessorInstance(Recipe recipe, ProcessorData data, int inventorySize, Vector2Int pos) : base(inventorySize, Vector2IntToGuid(pos))
        {
            _data = data;
            Recipe = recipe;
        }
        
        
        public ProcessorInstance(Recipe recipe, ProcessorData data, int inventorySize, Guid id) : base(inventorySize, id)
        {
            _data = data;
            Recipe = recipe;
        }

        public Recipe Recipe { get; }

        public override event Action<Resource> OnResourceProduced;
        
        public override void ProcessTicks(int deltaTicks = 1)
        {
            if (Recipe == null) return;

            if (!_processing && InputAmountSatisfied())
            {
                ConsumeInputs();
                _processing = true;
                ticks = 0;
                
            }

            if (!_processing) return;
            
            ticks += (int)(deltaTicks * _data.Speed);
            if (!(ticks >= Recipe.Ticks)) {
                return;
            }
                
            ProduceOutputs();
            _processing = false;
            ticks = 0;
        }

        private bool InputAmountSatisfied()
        {
            foreach (var r in Recipe.InputVolumes)
            {
                if (!InputResources.ContainsKey(r.resource) || InputResources[r.resource] < r.amount)
                    return false;
            }
            return true;
        }

        private void ConsumeInputs()
        {
            foreach (var r in Recipe.InputVolumes)
            {
                InputResources[r.resource] -= r.amount;
            }
        }

        private void ProduceOutputs()
        {
            if (OutputResources.resource != Recipe.OutputVolume.resource)
            {
                OutputResources.resource = Recipe.OutputVolume.resource;
                OutputResources.amount = 0;
            }
            
            OnResourceProduced?.Invoke(OutputResources.resource);
            OutputResources.amount += Recipe.OutputVolume.amount;
        }

    }
}