using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JSM.Surveillance.Game;
using JSM.Surveillance.Saving;
using JSM.Surviellance.Saving;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JSM.Surveillance
{
    public partial class InputMachineInstance
    {
        public string GetDataId() {
            return Data.AssetGuid;
        }
        
        public override MachineStateDto BuildMachineDTO()
        {
            return new InputInstanceDTO(this);
        }
    }
    
    [System.Serializable]
    public class InputInstanceDTO : MachineStateDto, ISourceDependent
    {
        [SerializeField] 
        private SerializableGuid _sourceId;

        public Guid SourceId 
        {
            get => _sourceId; // Implicitly converts to System.Guid
            set => _sourceId = value; // Implicitly converts to SerializableGuid
        }
        private Source _source;

        public InputInstanceDTO() : base() { }

        public InputInstanceDTO(InputMachineInstance inputMachineInstance) 
            : base(inputMachineInstance) 
        {
            DtoType = DtoType.InputInstance;
            _sourceId = inputMachineInstance.Source.GetGuid();
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(SourceId.ToByteArray());
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            SourceId = new Guid(reader.ReadBytes(16));
        }

        public void RehydrateSourceReferences(Dictionary<Guid, Source> sl)
        {
            if (sl.TryGetValue(SourceId, out var value)) {
                _source = value; 
            }
            else {
                #if UNITY_EDITOR
                Debug.LogWarning($"[SaveSystem] Could not find Source with ID {_sourceId} for an Input Machine.");
                #endif
            }
        }

        public override async Task<MachineInstance> BuildMachineInstance()
        {
            return new InputMachineInstance(_source, InventorySize, Id);
        }
    }
}