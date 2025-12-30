using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.Game;
using UnityEngine;
using UnityEngine.Events;

namespace JSM.Surveillance
{
    [RequireComponent(typeof(MoneyManager))]
    public class SurveillanceGameManager : MonoBehaviour
    {
        private MapCellManager _mapCellManager;
        public static SurveillanceGameManager instance { get; private set; }
        private MoneyManager _moneyManager;
        
        [Tooltip("Used in case source doesn't have grid.")]
        [SerializeField]private FactoryGrid defaultSourceGrid;

        public MoneyManager MoneyManager => _moneyManager;
        public FactoryGrid DefaultSourceGrid => defaultSourceGrid; 

        private void Awake() {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
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
            
            SpawnSource(data);
            return true;
        }

        private static void SpawnSource(SourceData data)
        {
            var obj = Instantiate(data.Source.gameObject);
            obj.GetComponent<Source>().Init(FindObjectOfType<MapCellManager>(), data);
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
    }
}
