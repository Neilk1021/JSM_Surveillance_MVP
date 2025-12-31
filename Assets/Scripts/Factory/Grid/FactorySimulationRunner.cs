using System;
using JSM.Surveillance.Game;
using UnityEngine;

namespace JSM.Surveillance
{
    public class FactorySimulationRunner : MonoBehaviour
    {
        [SerializeField] private Source _source;
        [SerializeField] private float secondsPerTick = 1/5f;


        private float _timeSinceLastTick = 0;
        private FactoryGridSimulation _simulation;

        private bool _run = false;

        private void Awake()
        {
            _source = GetComponent<Source>();
        }

        public void Run()
        {
            _run = true;
        }
        
        private void Update()
        {
            if(!_run) return;
            _timeSinceLastTick += Time.deltaTime;
            if (_timeSinceLastTick > secondsPerTick)
            {
                _simulation?.RunTick();
                _timeSinceLastTick = 0;
            }
        }

        public void Load(FactoryGridSimulation simulation)
        {
            _simulation = simulation;
        }
    }
}