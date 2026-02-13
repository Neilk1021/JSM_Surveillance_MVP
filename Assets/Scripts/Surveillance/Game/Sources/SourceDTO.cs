using System;
using System.Collections.Generic;
using System.IO;
using JSM.Surveillance.Saving;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance.Game
{
    public enum SourceDTOType {
        Normal= 0,
        Camera = 1,
        Mic = 2
    }
    
   [Serializable]
    public class SourceDTO
    {
        public SourceDTOType SourceDtoType = SourceDTOType.Normal;
        [SerializeField] private string _guidString;
        public Guid Guid
        {
            get => Guid.TryParse(_guidString, out var g) ? g : Guid.Empty;
            set => _guidString = value.ToString();
        }
        
        public string sourceName;
        public Vector3 position; 
        public SimulationSaveData Simulation;
        public FactoryBlueprint lastLayout;
        public string sourceDataPath;
        public Guid NextSource;
        public List<Guid> IncomingSources = new List<Guid>();

        public SourceDTO()
        {
        }

        public SourceDTO(SourceDTO sourceDto)
        {
            Guid = sourceDto.Guid;
            sourceName = sourceDto.sourceName;
            position = sourceDto.position;
            Simulation = sourceDto.Simulation;
            lastLayout = sourceDto.lastLayout;
            sourceDataPath = sourceDto.sourceDataPath;
            NextSource = sourceDto.NextSource;
            IncomingSources = sourceDto.IncomingSources;
        }
        
        public virtual void Write(BinaryWriter writer)
        {
            writer.Write((byte)SourceDtoType);
            writer.Write(Guid.ToByteArray());
            writer.Write(sourceName ?? ""); 
            writer.Write(position.x);
            writer.Write(position.y);
            writer.Write(position.z); 
            
            writer.Write(NextSource.ToByteArray());
    
            writer.Write(IncomingSources?.Count ?? 0);
            if (IncomingSources != null)
            {
                foreach(var id in IncomingSources)
                    writer.Write(id.ToByteArray());
            }

            writer.Write(sourceDataPath ?? "");
            
            writer.Write(Simulation != null);
            Simulation?.Write(writer);

            writer.Write(lastLayout != null);
            lastLayout?.Write(writer);
        }

        public virtual void Read(BinaryReader reader)
        {
            Guid = new Guid(reader.ReadBytes(16));
            sourceName = reader.ReadString(); 
            
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            position = new Vector3(x, y, z);
            
            NextSource = new Guid(reader.ReadBytes(16));

            int incomingCount = reader.ReadInt32();
            IncomingSources = new List<Guid>(incomingCount);
            for(int i = 0; i < incomingCount; i++)
                IncomingSources.Add(new Guid(reader.ReadBytes(16)));

            sourceDataPath = reader.ReadString();
            
            if (reader.ReadBoolean()) 
            {
                Simulation = new SimulationSaveData();
                Simulation.Read(reader);
            }

            if (reader.ReadBoolean()) 
            {
                lastLayout = new FactoryBlueprint();
                lastLayout.Read(reader);
            }
        }

        public class CameraDTO : SourceDTO
        {
            public float Angle;
            
            public CameraDTO() {}

            public CameraDTO(SourceDTO sourceDto) : base(sourceDto)
            {
                SourceDtoType = SourceDTOType.Camera;
            }

            public override void Write(BinaryWriter writer)
            {
                SourceDtoType = SourceDTOType.Camera;
                base.Write(writer);
                writer.Write(Angle);
            }

            public override void Read(BinaryReader reader)
            {
                base.Read(reader);
                Angle = reader.ReadSingle();
            }
        }

        public class MicDTO : SourceDTO
        {
            public Vector3 CenterPos;

            public MicDTO() {}
            
            public MicDTO(SourceDTO sourceDto) : base(sourceDto) {
                SourceDtoType = SourceDTOType.Mic;
            }
            
            public override void Write(BinaryWriter writer)
            {
                SourceDtoType = SourceDTOType.Mic;
                
                base.Write(writer);
                
                writer.Write(CenterPos.x);
                writer.Write(CenterPos.y);
                writer.Write(CenterPos.z);
            }

            public override void Read(BinaryReader reader)
            {
                base.Read(reader);
                CenterPos = new Vector3();
                CenterPos.x = reader.ReadSingle();
                CenterPos.y = reader.ReadSingle();
                CenterPos.z = reader.ReadSingle();
            }
        }
    }
    
    
    
}