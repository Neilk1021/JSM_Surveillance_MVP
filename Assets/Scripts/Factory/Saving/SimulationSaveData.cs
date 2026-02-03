using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JSM.Surviellance.Saving;
using UnityEngine;

namespace JSM.Surveillance.Saving
{
    
    public class SimulationSaveData
    {
        public readonly List<MachineStateDto> MachineStates = new List<MachineStateDto>();
        public readonly List<ConnectionData> Connections = new List<ConnectionData>(); 
        
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

        private MachineStateDto BuildMachine(BinaryReader reader)
        {
            DtoType type = (DtoType)reader.ReadByte();
            switch (type)
            {
                case DtoType.ProcessorInstance:
                    var processor = new ProcessorInstanceDTO();
                    processor.Read(reader);
                    return processor;
                default:
                    throw new ArgumentException("Tried to instantiate non valid Machine on file load");
                    break;
            }
        }
    }

    public class ConnectionData
    {
        public Guid StartMachineID;
        public Guid EndMachineID;
    }

    public enum DtoType
    {
        Base = 0,
        ProcessorInstance = 1,
        //continue as we add more
    }
    
    public abstract class MachineStateDto
    {
        protected DtoType DtoType { get; set; }
        public Guid Id { get; private set; }
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