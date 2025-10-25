using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JSM.Surveillance
{
    public class UnlockedMachines : MonoBehaviour
    {
        private Dictionary<Maintainable, bool> _machines;

        /// <summary>
        /// Unlocks a given maintainable object.
        /// </summary>
        /// <param name="machineToUnlock">Machine to unlock</param>
        /// <returns>True if machine was unlocked, false if failed to unlock.</returns>
        bool UnlockMachine(Maintainable machineToUnlock)
        {
            if (!_machines.ContainsKey(machineToUnlock) || _machines[machineToUnlock] == true) {
                return false;
            }
            
            _machines[machineToUnlock] = true;
            return true;
        }
        
        /// <summary>
        /// Returns all Maintainable objects that the player has unlocked.
        /// </summary>
        /// <returns>List of maintainable objects.</returns>
        List<Maintainable> GetUnlockedMachines()
        {
            return _machines.Where(e => e.Value == true).Select(e=>e.Key).ToList();
        }
    }
}