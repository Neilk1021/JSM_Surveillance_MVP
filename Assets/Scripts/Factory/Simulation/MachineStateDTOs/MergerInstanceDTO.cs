using System.IO;
using System.Threading.Tasks;
using JSM.Surveillance.Saving;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JSM.Surveillance
{
    public partial class MergerInstance 
    {
        public string GetDataId() {
            return _data.AssetGuid;
        }
        
        public override MachineStateDto BuildMachineDTO() {
            return new MergerInstanceDTO(this);
        }

    }
    
    public class MergerInstanceDTO : MachineStateDto
    {
        private string _dataID;
        
        public MergerInstanceDTO() : base(){}
        
        public MergerInstanceDTO(MergerInstance mergerInstance) : base(mergerInstance) {
            DtoType = DtoType.MergerInstance;
            _dataID = mergerInstance.GetDataId();
            
        }

        public override async Task<MachineInstance> BuildMachineInstance() {
            var pData = Addressables.LoadAssetAsync<ProcessorData>(_dataID);
            await pData.Task;


            if (pData.Status == AsyncOperationStatus.Succeeded)
            {
                return new MergerInstance(InventorySize, Id, pData.Result);
            }

            return null;
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(_dataID);
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            _dataID = reader.ReadString();
        }
        
    }
}