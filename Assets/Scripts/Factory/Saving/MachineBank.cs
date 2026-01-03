using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance.Saving
{
    
    [CreateAssetMenu(fileName = "NewMachineBank", menuName = "JSM/Surveillance/MachineBank")]
    public class MachineBank : ScriptableObject
    {
        [FormerlySerializedAs("_machinePrefabs")] [SerializeField] private List<MachineObject> machinePrefabs;

        public MachineObject GetMachine(string prefabId)
        {
            var machine = machinePrefabs.FirstOrDefault(x => x.Guid.ToString() == prefabId);
            return machine;
        }
    }
}