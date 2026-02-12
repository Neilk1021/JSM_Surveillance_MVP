using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace JSM.Surveillance.Saving
{
    [System.Serializable]
    public class FactoryBlueprint
    {
        public List<MachineNode> machines = new();
        public List<ConnectionEdge> connections = new();
        public int TotalCost { get; set; }
        
        public void Write(BinaryWriter writer)
        {
            writer.Write(machines.Count);
            foreach (var machine in machines)
            {
                writer.Write(machine.prefabId ?? "");
                writer.Write(machine.recipeId ?? "");
                writer.Write(machine.rotation);
                writer.Write(machine.localPos.x);
                writer.Write(machine.localPos.y);
                writer.Write(machine.localPos.z);

                writer.Write(machine.positions?.Count ?? 0);
                if (machine.positions != null)
                {
                    foreach (var p in machine.positions)
                    {
                        writer.Write(p.x);
                        writer.Write(p.y);
                    }
                }
            }

            writer.Write(connections.Count);
            foreach (var conn in connections)
            {
                writer.Write(conn.fromPos.x);
                writer.Write(conn.fromPos.y);
                writer.Write(conn.toPos.x);
                writer.Write(conn.toPos.y);
                writer.Write(conn.fromRootPos.x);
                writer.Write(conn.fromRootPos.y);
                writer.Write(conn.toRootPos.x);
                writer.Write(conn.toRootPos.y);
                writer.Write(conn.inputPortIndex);

                writer.Write(conn.positions?.Length ?? 0);
                if (conn.positions != null)
                {
                    foreach (var p in conn.positions)
                    {
                        writer.Write(p.x);
                        writer.Write(p.y);
                    }
                }
            }
        }

        public void Read(BinaryReader reader)
        {
            machines.Clear();
            int machineCount = reader.ReadInt32();
            for (int i = 0; i < machineCount; i++)
            {
                var machine = new MachineNode();
                machine.prefabId = reader.ReadString();
                machine.recipeId = reader.ReadString();
                machine.rotation = reader.ReadInt32();

                machine.localPos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                int posCount = reader.ReadInt32();
                machine.positions = new List<Vector2Int>(posCount);
                for (int j = 0; j < posCount; j++)
                {
                    machine.positions.Add(new Vector2Int(reader.ReadInt32(), reader.ReadInt32()));
                }

                machines.Add(machine);
            }

            connections.Clear();
            int connectionCount = reader.ReadInt32();
            for (int i = 0; i < connectionCount; i++)
            {
                var conn = new ConnectionEdge();
                conn.fromPos = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
                conn.toPos = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
                conn.fromRootPos = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
                conn.toRootPos = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
                conn.inputPortIndex = reader.ReadInt32();

                int pathCount = reader.ReadInt32();
                conn.positions = new Vector2Int[pathCount];
                for (int j = 0; j < pathCount; j++)
                {
                    conn.positions[j] = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
                }

                connections.Add(conn);
            }
        }

    }

    [System.Serializable]
    public class MachineNode {
        public List<Vector2Int> positions;
        public string prefabId; 
        public string recipeId;
        public Vector3 localPos;
        public int rotation;
    }

    [System.Serializable]
    public class ConnectionEdge
    {
        public Vector2Int fromPos;
        public Vector2Int toPos;
        public Vector2Int toRootPos;
        public Vector2Int fromRootPos;
        public Vector2Int[] positions;
        public int inputPortIndex;
    }
}