using System;
using Surveillance.TechTree;
using UnityEngine;

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "MachineSObj", menuName = "JSM/Surveillance/MachineSObj")]
    public class MachineSObj : Maintainable 
    {
        [SerializeField] private MachineObject machineObject;
        public MachineObject MachineObject => machineObject;
    }
}