using System.Collections;
using System.Collections.Generic;

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