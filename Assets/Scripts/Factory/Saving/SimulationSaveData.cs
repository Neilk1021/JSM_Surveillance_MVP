using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JSM.Surveillance.Game;
using JSM.Surviellance.Saving;
using UnityEngine;

namespace JSM.Surveillance.Saving
{
    [System.Serializable]
    public class SimulationSaveData
    {
        [SerializeReference]
        public List<MachineStateDto> MachineStates = new List<MachineStateDto>();
        [SerializeReference]
        public List<ConnectionData> Connections = new List<ConnectionData>();

        public bool Rehydrated = false;
        
        public SimulationSaveData(){}
        
        
        public SimulationSaveData(List<MachineStateDto> machineStateDtos, List<ConnectionData> connectionData)
        {
            MachineStates = machineStateDtos;
            Connections = connectionData;
        }

        public void Write(BinaryWriter writer)
        {
            try
            {
                writer.Write(MachineStates.Count);
                foreach (var dto in MachineStates)
                {
                    dto.Write(writer);
                }

                writer.Write(Connections.Count);
                foreach (var conn in Connections)
                {
                    writer.Write(conn.StartMachineID.ToByteArray());
                    writer.Write(conn.EndMachineID.ToByteArray());
                }
            }
            catch (UnauthorizedAccessException e)
            {
                throw new SaveFailedException("Access violation while saving: ", e);
            }
            catch (Exception e)
            {
                throw new SaveFailedException("Save failed due to: ", e);
            }

        }

        public void Read(BinaryReader reader)
        {
            MachineStates.Clear();
            Connections.Clear();
            Rehydrated = false;
            try
            {
                int machineStates = reader.ReadInt32();
                for (int i = 0; i < machineStates; ++i)
                {
                    var m = BuildMachine(reader);
                    MachineStates.Add(m);
                }

                int connections = reader.ReadInt32();
                for (int i = 0; i < connections; ++i)
                {
                    byte[] startingGuid = reader.ReadBytes(16);
                    byte[] endingGuid = reader.ReadBytes(16);

                    Connections.Add(new ConnectionData()
                    {
                        StartMachineID = new Guid(startingGuid),
                        EndMachineID = new Guid(endingGuid)
                    });
                }
                
            }
            catch (EndOfStreamException e)
            {
                MachineStates.Clear();
                Connections.Clear();

                throw new LoadFailedException("Load failed, cause: ", e);
            }
            
            catch (Exception e)
            {
                MachineStates.Clear();
                Connections.Clear();
                throw new LoadFailedException("Load failed due to: ", e);
            }
        }

        public void RehydrateSourceReferences(Dictionary<Guid, Source> sourceLookup) {
            try
            {
                foreach (var machineState in MachineStates) {
                    if (machineState is ISourceDependent sourceDependent) {
                        sourceDependent.RehydrateSourceReferences(sourceLookup);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
            Rehydrated = true;
        }

        private MachineStateDto BuildMachine(BinaryReader reader)
        {
            DtoType type = (DtoType)reader.ReadByte();
            switch (type)
            {
                case DtoType.ProcessorInstance:
                    var processor = new ProcessorInstanceDTO();
                    processor.Read(reader);
                    return processor;
                case DtoType.MergerInstance:
                    var merger = new MergerInstanceDTO();
                    merger.Read(reader);
                    return merger;
                case DtoType.OutputInstance:
                    var output = new OutputInstanceDTO();
                    output.Read(reader);
                    return output;
                case DtoType.InputInstance:
                    var input = new InputInstanceDTO();
                    input.Read(reader);
                    return input;
                case DtoType.ExternalInput:
                    var externalInput = new ExternalInstanceDTO();
                    externalInput.Read(reader);
                    return externalInput;
                default:
                    throw new ArgumentException("Tried to instantiate non valid Machine on file load");
                    break;
            }
        }
    }
    [System.Serializable]
    public class ConnectionData : ISerializationCallbackReceiver
    {
        public Guid StartMachineID;
        public Guid EndMachineID;

        [SerializeField, HideInInspector] private string _startMachineIDStr;
        [SerializeField, HideInInspector] private string _endMachineIDStr;

        public void OnBeforeSerialize()
        {
            _startMachineIDStr = StartMachineID.ToString();
            _endMachineIDStr = EndMachineID.ToString();
        }

        public void OnAfterDeserialize()
        {
            if (Guid.TryParse(_startMachineIDStr, out var startGuid))
                StartMachineID = startGuid;
        
            if (Guid.TryParse(_endMachineIDStr, out var endGuid))
                EndMachineID = endGuid;
        }
    }

    public enum DtoType
    {
        Base = 0,
        ProcessorInstance = 1,
        MergerInstance = 2,
        OutputInstance = 3,
        
        //cont
        InputInstance = 4,
        ExternalInput = 5
    }
    [Serializable]
    public struct SerializableGuid
    {
        [SerializeField] private string value;

        public SerializableGuid(Guid guid) => value = guid.ToString();

        // This allows you to treat SerializableGuid exactly like a System.Guid
        public static implicit operator Guid(SerializableGuid s) => 
            string.IsNullOrEmpty(s.value) ? Guid.Empty : new Guid(s.value);
    
        public static implicit operator SerializableGuid(Guid g) => 
            new SerializableGuid(g);
    }
    
    [System.Serializable]
    public class MachineStateDto
    {
        [SerializeField]
        protected DtoType DtoType { get; set; }
        [SerializeField]
        private string _id; 

        public Guid Id 
        {
            get => Guid.Parse(_id);
            private set => _id = value.ToString();
        }
        protected int InventorySize { get; private set; }

        protected MachineStateDto() { }
        
        protected MachineStateDto(MachineInstance machineInstance) {
            DtoType = DtoType.Base;
            InventorySize = machineInstance.InventorySize;
            Id = machineInstance.Guid;
        }

        public virtual async Task<MachineInstance> BuildMachineInstance()
        {
            return null;
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write((byte)DtoType);
            writer.Write(Id.ToByteArray());
            writer.Write(InventorySize);
        }

        public virtual void Read(BinaryReader reader)
        {
            byte[] idBytes = reader.ReadBytes(16);
            Id = new Guid(idBytes);
            InventorySize = reader.ReadInt32();
        }
    }

}