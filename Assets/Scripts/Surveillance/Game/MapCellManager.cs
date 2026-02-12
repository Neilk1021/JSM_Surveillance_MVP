using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JSM.Surveillance.Surveillance;
using JSM.Surveillance.UI;
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
        private SourceUI _currentSourceUI = null;

        private void Awake() {
            _cells = FindObjectsOfType<MapCell>().ToDictionary(x => x.Face);
            _mapCellDataManager = GetComponent<MapCellDataManager>();
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

        public IEnumerable<HEFace> GetFacesAroundPoint(Vector3 transformPosition, int depth = 4) {
            return _mapCellDataManager.GetFacesAroundPoint(transformPosition, depth);
        }

        public IList<Vector2> GetFacePoints(HEFace face) {
            return _mapCellDataManager.GetFacePoints(face);
        }

        
        public void CloseUIPreview()
        {
            if (_currentSourceUI == null) return;
            
            Destroy(_currentSourceUI.gameObject);
            _currentSourceUI = null;
        } 
        
        public void SwitchUIPreview(Source source) {
            if (_currentSourceUI != null && source == _currentSourceUI.GetSource()) {
                CloseUIPreview();
                return;
            }
            
            CloseUIPreview();
            _currentSourceUI = source.CreateUI();
        }

        public void SetMapMode(MapMode mode)
        {
            MapRenderingManager.instance.ChangeRendering(mode);
        }

        public void SetFacePlacementPct(HEFace faceKey, float faceValue)
        {
            if (!_cells.ContainsKey(faceKey))
            {
                return;
            }
            _cells[faceKey].Rendering.SetAlpha(faceValue);
        }

        public float GetResourceRatioInFace(HEFace face, Resource resource)
        {
            return resource.ResourceCategory switch
            {
                ResourceCategory.Consumer => face.Data.ratio.consumer,
                ResourceCategory.Corp => face.Data.ratio.corporate,
                ResourceCategory.Govt => face.Data.ratio.government,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void SetAllFacePlacementPct(float i)
        {
            foreach (var cell in _cells.Values)
            {
                cell.Rendering.SetAlpha(i);
            }
        }
    }
}