using UnityEngine;

namespace JSM.Surveillance
{
    public class ProcessorInstance : MachineInstance
    {
        private ProcessorData _data;
        private int ticks = 0;
        private bool _processing = false;

        public ProcessorInstance(Recipe recipe, ProcessorData data, int inventorySize) : base(inventorySize)
        {
            _data = data;
            Recipe = recipe;
        }

        public Recipe Recipe { get; }
        public bool IsRunning { get; } = false;

        
        public override void ProcessTicks(int deltaTicks = 1)
        {
            if (Recipe == null) return;

            if (!_processing && InputAmountSatisfied())
            {
                ConsumeInputs();
                _processing = true;
                ticks = 0;
                
                Debug.Log($"On this tick I started processing!");
            }
            if (_processing)
            {
                ticks += (int)(deltaTicks * _data.Speed);

                if (!(ticks >= Recipe.Ticks)) {
                    Debug.Log($"On this tick, ticks were lower than needed to make!");
                    return;
                }
                
                Debug.Log($"On this tick I produced 1 AA.");
                ProduceOutputs();
                _processing = false;
                ticks = 0;
            }
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
            
            OutputResources.amount += Recipe.OutputVolume.amount;
        }

    }
}