using System;
using UnityEngine;

namespace JSM.Surveillance.Game
{
    public abstract partial class Source
    {
        [Header("Grid [Leave blank for default]")]
        [SerializeField] private FactoryGrid gridPrefab;

        private FactoryGrid _grid = null;
        
        protected virtual void Awake() {
            gridPrefab ??= SurveillanceGameManager.instance.DefaultSourceGrid;
        }
        
        public void SpawnGrid()
        {
            if (_grid != null) {
                //TODO, make it show in front
                return;
            }

            _grid = Instantiate(gridPrefab, transform.position, Quaternion.identity);
        }
    }
}