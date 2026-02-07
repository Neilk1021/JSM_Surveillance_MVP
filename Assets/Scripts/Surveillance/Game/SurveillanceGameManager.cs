using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private List<Source> _sources = new List<Source>();

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

            SpawnSource(data);
            
            return true;
        }
        public static Source SpawnSourceImmediate(SourceData data)
        {
            var obj = Instantiate(data.Source.gameObject);
            var spawnSource = obj.GetComponent<Source>();
            spawnSource.Init(FindObjectOfType<MapCellManager>(), data, true);
            instance._sources.Add(spawnSource);
            return spawnSource;
        }


        
        public static Source SpawnSource(SourceData data)
        {
            var obj = Instantiate(data.Source.gameObject);
            var spawnSource = obj.GetComponent<Source>();
            spawnSource.Init(FindObjectOfType<MapCellManager>(), data);
            instance._sources.Add(spawnSource);
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

        public static void ClearBoard()
        {
            var sourcesToClear = instance._sources;
            instance._sources = new List<Source>();

            instance.ClearSourcesBackground(sourcesToClear);
        }

        private void ClearSourcesBackground(List<Source> sources)
        {
            StartCoroutine(ClearSourcesBackgroundC(sources));
        }
        
        private IEnumerator ClearSourcesBackgroundC(List<Source> sources, int batchSize = 10)
        {
            foreach(var s in sources) s.gameObject.SetActive(false);

            for (int i = 0; i < sources.Count; i++)
            {
                var s = sources[i];
                if (s && s.gameObject)
                {
                    Destroy(s.gameObject);
                }

                if (i > 0 && i % batchSize == 0) 
                    yield return null;
            }
        }

        public static int GetMoney()
        {
            return instance._moneyManager.Money;
        }

        public static void SetMoney(int money)
        {
            instance._moneyManager.SetMoney(money);
        }
    }
}
