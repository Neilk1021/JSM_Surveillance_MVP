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
    }
}