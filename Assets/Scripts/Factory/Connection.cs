using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [System.Serializable]
    public class Connection
    {
        [SerializeField] private Maintainable inputMachine;
        [SerializeField] private Maintainable outputMachine;

        public Maintainable InputMachine => inputMachine;
        public Maintainable OutputMachine => outputMachine;


    }
}
