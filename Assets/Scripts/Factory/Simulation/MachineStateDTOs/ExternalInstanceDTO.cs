using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JSM.Surveillance.Game;
using JSM.Surveillance.Saving;
using JSM.Surviellance.Saving;
using UnityEngine;

namespace JSM.Surveillance
{

    public partial class ExternalInputInstance
    {
        public override MachineStateDto BuildMachineDTO()
        {
            return new ExternalInstanceDTO(this);
        }
    }
    
    public class ExternalInstanceDTO : MachineStateDto, ISourceDependent
    {
        private Guid _sourceId;
        private Source _source;

        private int incomingSourceIndex; 

        public ExternalInstanceDTO() : base() { }

        public ExternalInstanceDTO(ExternalInputInstance externalMachineInstance) 
            : base(externalMachineInstance) 
        {
            DtoType = DtoType.ExternalInput;
            incomingSourceIndex = externalMachineInstance.IncomingSourceIndex;
            _source = externalMachineInstance.Source;
            _sourceId = externalMachineInstance.Source != null ? externalMachineInstance.Source.GetGuid() : Guid.Empty;
            
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(_sourceId.ToByteArray());
            writer.Write(incomingSourceIndex);
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            _sourceId = new Guid(reader.ReadBytes(16));
            incomingSourceIndex = reader.ReadInt32();
        }

        public void RehydrateSourceReferences(Dictionary<Guid, Source> sl)
        {
            if (_sourceId == Guid.Empty) {
                _source = null;
                return;
            }
            
            if (sl.TryGetValue(_sourceId, out var value)) {
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
            return new ExternalInputInstance(_source, incomingSourceIndex, InventorySize, Id);
        }
    }
}