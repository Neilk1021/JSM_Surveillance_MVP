using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Game;
using UnityEngine;
using UnityEngine.Events;

namespace JSM.Surveillance
{
    [RequireComponent(typeof(Simulator))]
    [RequireComponent(typeof(MoneyManager))]
    public class SurveillanceGameManager : MonoBehaviour
    {
        private MapCellManager _mapCellManager;
        public static SurveillanceGameManager instance { get; private set; }
        private MoneyManager _moneyManager;
        private Simulator _simData;
        
        [Tooltip("Used in case source doesn't have grid.")]
        [SerializeField]private FactoryGrid defaultSourceGrid;

        public Simulator Simulator => _simData;
        public MoneyManager MoneyManager => _moneyManager;
        public FactoryGrid DefaultSourceGrid => defaultSourceGrid;
        private readonly List<Source> _sources = new List<Source>();

        private void Awake() {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            _simData = GetComponent<Simulator>();
            _moneyManager = GetComponent<MoneyManager>();
            _mapCellManager = FindObjectOfType<MapCellManager>();
        }
        
        public bool BuySource(SourceData data)
        {
            CheckSourceDataValidity(data);
            if (!_moneyManager.ChangeMoneyBy(-data.UpfrontCost))
            {
                return false;
            }
            
            _sources.Add(SpawnSource(data));
            return true;
        }

        private static Source SpawnSource(SourceData data)
        {
            var obj = Instantiate(data.Source.gameObject);
            var spawnSource = obj.GetComponent<Source>();
            spawnSource.Init(FindObjectOfType<MapCellManager>(), data);
            return spawnSource;
        }

        private static void CheckSourceDataValidity(SourceData data)
        {
            if (data == null) {
                throw new ArgumentException("Data was not provided.");
            }

            if (data.Source == null) {
                throw new ArgumentException("Data has no source");
            }
        }

        public void SellSource(Source source)
        {
            _moneyManager.ChangeMoneyBy(source.Data.UpfrontCost);
            
            source.Destroy();
        }

        public Source GetSourceClosetTo(Vector3 pos, float threshold)
        {
            if (_sources.Count <= 0) {
                throw new ArgumentException("No sources in the scene!");
            }


            return _sources 
                .Where(x => x)
                .Where(x=> Vector2.Distance(pos,x.transform.position) < threshold)
                .OrderBy(x=> Vector2.Distance(pos, x.transform.position))
                .FirstOrDefault();        
        }
    }
}
