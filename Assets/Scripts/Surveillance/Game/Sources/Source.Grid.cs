using System;
using JSM.Surveillance.Saving;
using UnityEngine;

namespace JSM.Surveillance.Game
{
    public abstract partial class Source
    {
        [Header("Grid [Leave blank for default]")]
        [SerializeField] private FactoryGrid gridPrefab;

        private FactoryGrid _grid = null;
        [SerializeField] public Resource Resource;
        public FactoryGrid Grid => _grid;
        private FactoryGridSimulation _simulation;
        private FactorySimulationRunner _factorySimulationRunner;
        private FactoryBlueprint _lastLayout;
        

        protected virtual void Awake()
        {
            _factorySimulationRunner = GetComponent<FactorySimulationRunner>();
            gridPrefab ??= SurveillanceGameManager.instance.DefaultSourceGrid;
        }
        
        public void SpawnGrid()
        {
            if (_grid != null) {
                //TODO, make it show in front
                return;
            }

            if (Camera.main != null)
            {
                _grid = Instantiate(gridPrefab, GameObject.FindGameObjectWithTag("FactoryCam").transform);
                _grid.SetSource(this);
                _grid.transform.localPosition = new Vector3(0, -6.5f, 1.25f);
                _grid.Initialize(_lastLayout);
            }
        }

        public void HandleOutputResourceVolume(OutputMachineInstance machine)
        {
            foreach (var machineOutputResource in machine.OutputResources)
            {
                SurveillanceGameManager.instance.MoneyManager.ChangeMoneyBy(machineOutputResource.Value *
                                                                            machineOutputResource.Key.Value);
                Debug.Log(machineOutputResource.Value);
            }

            machine.OutputResources.Clear();
        }

        public void SetSimulation(FactoryGridSimulation buildSimulator)
        {
            _simulation = buildSimulator;
        }

        public void RunSimulator()
        {
            _factorySimulationRunner.Load(_simulation);
            _factorySimulationRunner.Run();
        }

        public void SetLastLayout(FactoryBlueprint newLayout)
        {
            _lastLayout = newLayout;
        }
    }
}