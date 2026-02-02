using System;
using System.Collections.Generic;
using System.IO;

namespace JSM.Surveillance.Saving
{
    public class SimulationSaveData
    {
        public List<MachineStateDto> MachineStates { get; set; }
        public List<ConnectionData> Connections { get; set; }
    }

    public class ConnectionData
    {
        public Guid StartMachineID;
        public Guid EndMachineID;
    }

    public abstract class MachineStateDto
    {
        public Guid Id { get; set; }
        
        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(Id.ToByteArray());
        }

        public virtual void Read(BinaryReader reader)
        {
            
        }
    }
}