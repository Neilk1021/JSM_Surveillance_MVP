using System.IO;
using System.Threading.Tasks;
using JSM.Surveillance.Saving;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JSM.Surveillance
{
    public partial class ProcessorInstance
    {
        public string GetDataId() {
            return _data.AssetGuid;
        }
        
        public override MachineStateDto BuildMachineDTO() {
            return new ProcessorInstanceDTO(this);
        }
    }


    public class ProcessorInstanceDTO : MachineStateDto
    {
        private string _recipeID;
        private string _dataID;

        public ProcessorInstanceDTO() : base(){}
        
        public ProcessorInstanceDTO(ProcessorInstance processorInstance) : base(processorInstance) {
            DtoType = DtoType.ProcessorInstance;
            _recipeID = processorInstance.Recipe.Guid;
            _dataID = processorInstance.GetDataId();
            
        }

        public override async Task<MachineInstance> BuildMachineInstance() {
            var pData = Addressables.LoadAssetAsync<ProcessorData>(_dataID);
            await pData.Task;

            var rData = Addressables.LoadAssetAsync<Recipe>(_recipeID);
            await rData.Task;

            if (pData.Status == AsyncOperationStatus.Succeeded && rData.Status == AsyncOperationStatus.Succeeded)
            {
                return new ProcessorInstance(rData.Result, pData.Result, InventorySize, Id);
            }

            return null;
        }

        public override void Write(BinaryWriter writer)
        {
           base.Write(writer);
           writer.Write(_recipeID);
           writer.Write(_dataID);
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            _recipeID = reader.ReadString();
            _dataID = reader.ReadString();
        }
    }
}