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

            if (Camera.main != null)
            {
                _grid = Instantiate(gridPrefab, GameObject.FindGameObjectWithTag("FactoryCam").transform);
                _grid.SetSource(this);
                _grid.transform.localPosition = new Vector3(0, -6.5f, 1.25f);
            }
        }
    }
}