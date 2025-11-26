using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JSM.Surveillance.Data;
using JSM.Surveillance.Surveillance;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance.Game
{
    public class MapCellManager : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] GameObject companyTextPrefab;

        public GameObject CompanyTextPrefab => companyTextPrefab;

        private Dictionary<HEFace, MapCell> _cells;

        private MapCellDataManager _mapCellDataManager;

        private void Awake() {
            _cells = FindObjectsOfType<MapCell>().ToDictionary(x => x.Face);
            _mapCellDataManager = GetComponent<MapCellDataManager>();
        }

        public async Task PlaceSource(Source source)
        {
            //check if ts placed.
        }
        
        public async Task<bool> BuySource(Source source)
        {
            //do some shit

            await PlaceSource(source);

            return true;
        }
        
        
        public int GetPopulationInFace(HEFace face)
        {
            if (!_cells.ContainsKey(face))
            {
                return 0;
            }

            return _cells[face].GetData().DailyPopulation;
        }

        public Vector3 GetMouseCurrentPosition() {
            return _mapCellDataManager.GetMouseCurrentPosition();
        }

        public MapEdgeVertex GetVertexClosetTo(Vector3 mousePos, float f) {
            return _mapCellDataManager.GetVertexClosetTo(mousePos, f);
        }

        public IEnumerable<HEFace> GetFacesAroundPoint(Vector3 transformPosition) {
            return _mapCellDataManager.GetFacesAroundPoint(transformPosition);
        }

        public IList<Vector2> GetFacePoints(HEFace face) {
            return _mapCellDataManager.GetFacePoints(face);
        }
    }
}