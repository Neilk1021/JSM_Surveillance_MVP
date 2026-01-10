using System;
using JSM.Surveillance.Saving;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JSM.Surveillance.Game
{
    public abstract partial class Source
    {
        [Header("Grid [Leave blank for default]")]
        [SerializeField] private FactoryGrid gridPrefab;

        private FactoryGrid _grid = null;
        [FormerlySerializedAs("Resource")] [SerializeField] public Resource resource;
        public FactoryGrid Grid => _grid;

        private FactoryGridSimulation _simulation;
        private FactorySimulationRunner _factorySimulationRunner;
        private FactoryBlueprint _lastLayout;

        private ResourceVolume _storedResourceVolume;

        public event Action<int> OnMoneyEarned;
        public event Action<Resource> OnResourceMade;

        public static bool Editable => !SurveillanceGameManager.instance.Simulator.Running;

        protected virtual void Awake()
        {
            _factorySimulationRunner = GetComponent<FactorySimulationRunner>();
            gridPrefab ??= SurveillanceGameManager.instance.DefaultSourceGrid;
        }

        private void OnEnable()
        {
            SurveillanceGameManager.instance.Simulator.OnStart += ReloadSim;
        }

        private void OnDisable()
        {
            SurveillanceGameManager.instance.Simulator.OnStart -= ReloadSim;

        }

        
        private void ReloadSim()
        {
            if (_grid != null)
            {
                SetSimulation(_grid.BuildSimulator());
            }
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
                _grid.transform.localPosition = new Vector3(0, -5.25f, 1.25f);
                _grid.Initialize(_lastLayout);
                _grid.OnModify.AddListener(Modified);
            }
        }

        private void Modified()
        {
            OnModified?.Invoke();
        }

        private void SellOutput()
        {
            if(_storedResourceVolume.resource == null) return;

            var vol = TakeResources();
            OnMoneyEarned?.Invoke(vol.amount * vol.resource.Value);
            SurveillanceGameManager.instance.
                MoneyManager.ChangeMoneyBy(vol.amount * vol.resource.Value);
        }

        public ResourceVolume TakeResources()
        {
            var vol = new ResourceVolume()
            {
                resource = _storedResourceVolume.resource,
                amount =  _storedResourceVolume.amount
            };
            _storedResourceVolume.amount = 0;
            return vol;
        }

        public virtual void HandleOutputResourceVolume(OutputMachineInstance machine)
        {
            //broken cuz machines should only really store 1 thing
            foreach (var machineOutputResource in machine.OutputResources)
            {
                if (_storedResourceVolume.resource == machineOutputResource.Key)
                {
                    _storedResourceVolume.amount += machineOutputResource.Value;
                    continue;
                }

                _storedResourceVolume.resource = machineOutputResource.Key;
                _storedResourceVolume.amount = machineOutputResource.Value;
            }

            machine.OutputResources.Clear();

            if(_storedResourceVolume.amount == 0) return;
            
            OnResourceMade?.Invoke(_storedResourceVolume.resource);
            if (_nextSource == null) {
                SellOutput();
            }
        }

        public void SetSimulation(FactoryGridSimulation buildSimulator)
        {
            _simulation = buildSimulator;
            _simulation.ResourceMade += OnResourceMade;
            _factorySimulationRunner?.Load(_simulation);
            OnModified?.Invoke();
        }
        
        public void SetLastLayout(FactoryBlueprint newLayout)
        {
            _lastLayout = newLayout;
        }

        public Resource GetOutputResourceType()
        {
            if (_simulation == null && !_grid)
                return null;

            return _grid ? _grid.GetOutputResourceType() : _simulation?.GetOutputResourceType();
        }
        
        
        
    }
}