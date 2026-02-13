using System.Threading.Tasks;
using JSM.Surveillance.Saving;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JSM.Surveillance
{
    [System.Serializable]
    public class OutputInstanceDTO : MachineStateDto
    {
        public OutputInstanceDTO() : base() {}

        public OutputInstanceDTO(OutputMachineInstance outputMachineInstance) : base(outputMachineInstance) {
            DtoType = DtoType.OutputInstance;
        }
        
        public override async Task<MachineInstance> BuildMachineInstance() {
            return new OutputMachineInstance(InventorySize, Id);
        }

    }
}