using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JSM.Surveillance
{
    public class OutputMachine : MachineObject
    {
        public override MachineInstance BuildInstance()
        {
            return new OutputMachineInstance(inventorySize, GetRootPosition());
        }

        public override Resource GetResource()
        {
            if (InputPorts.Count == 0) return null;
            if (InputPorts.All(x => x.ConnectionObject == null)) return null;
            
            foreach (var iport in InputPorts)
            {
                if (iport.ConnectionObject == null) {
                    continue;
                }

                return iport.ConnectionObject.StartPortObject.Owner.GetResource();
            }

            return null;
        }
    }
}