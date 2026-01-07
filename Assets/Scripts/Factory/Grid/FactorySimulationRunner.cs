using System;
using JSM.Surveillance.Game;
using UnityEngine;

namespace JSM.Surveillance
{
    public class FactorySimulationRunner : MonoBehaviour
    {
        private FactoryGridSimulation _simulation;
        private Simulator _simulator;
        

        private void Awake()
        {
            _simulator = SurveillanceGameManager.instance.Simulator;
        }

        private void OnEnable()
        {
            _simulator ??= SurveillanceGameManager.instance.Simulator;
            _simulator.OnTick+= RunTick;
        }

        private void OnDisable()
        {
            if (_simulator == null) return;
            
            _simulator.OnTick -= RunTick;
        }


        private void RunTick(int ticks)
        {
            _simulation?.RunTick(ticks);
        }
        
        public void Load(FactoryGridSimulation simulation)
        {
            _simulation = simulation;
        }
    }
}