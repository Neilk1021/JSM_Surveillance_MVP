using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JSM.Surveillance.Surveillance;
using JSM.Surveillance.Util;
using UnityEngine;

namespace JSM.Surveillance.Game
{
    [RequireComponent(typeof(SeparatedAreaView))]
    public class MicSource : Source
    {
        private MapEdgeVertex vert = null;
        private SeparatedAreaView _areaView;
        private static int _micCount = 0;
        [SerializeField] private Transform micModel;
        
        
        private void Start()
        {
            _areaView = GetComponent<SeparatedAreaView>();
        }


        private void OnDestroy() {
            _micCount--;
        }

        public override void Init(MapCellManager manager, SourceData data, bool immediatePlace = false)
        {
            base.Init(manager, data);
            _micCount++;
            sourceName += $" {_micCount}";
            
            if (!immediatePlace)
            {
                MapCellManager.SetMapMode(MapMode.Corp);
            }
        }
        
        protected override void MoveSource()
        {
            var mousePos = MapCellManager.GetMouseCurrentPosition();
            if(mousePos.Equals(Vector3.negativeInfinity)) return;

            
            vert = MapCellManager.GetVertexClosetTo(mousePos, 0.5f);
            
            Vector3 currentPos;
            
            if (vert == null) {
                currentPos = mousePos;
            }
            else { 
                currentPos = vert.transform.position;
            }
            
            transform.position = new  Vector3(currentPos.x, currentPos.y, transform.position.z);
        }

        protected override void CheckIfPlaced()
        {
            if (Input.GetMouseButtonDown(0) && vert is not null)
            {
                if (vert.GetSource() != null) {
                    return;
                }
                
                Place(transform.position);
                vert.SetSource(this);
            }
        }

        IEnumerator SetPosition()
        {
            //throw new NotImplementedException("NEED TO SET LIMITS ON TS");
            
            Vector3 initialMousePosition = Input.mousePosition;
            float initialZRotation = transform.rotation.eulerAngles.z;

            var renderer = GetComponent<SeparatedAreaViewRenderer>();
            yield return new WaitForFixedUpdate();
            
            var lastMousePos =  MapCellManager.GetMouseCurrentPosition();
            while (true)
            {
                var mousePos = MapCellManager.GetMouseCurrentPosition();
                renderer.RefreshMesh(mousePos);
                if ((mousePos - lastMousePos).magnitude > 0.1f)
                {
                    lastMousePos = mousePos;
                    UpdateCenterPosition(mousePos, renderer);
                }
                
                
                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }

                yield return null;
            }

            MapCellManager.SetMapMode(MapMode.Normal);
        }

        private void UpdateCenterPosition(Vector3 mousePos, SeparatedAreaViewRenderer renderer)
        {
            renderer.RefreshMesh(mousePos);
            _areaView.SetCenter(mousePos);

            float rad = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
            micModel.rotation = Quaternion.Euler(micModel.eulerAngles.x, micModel.eulerAngles.y, -rad * Mathf.Rad2Deg-90);
                    
                    
            var faces = GetFacesInRange();
            foreach (var face in faces)
            {
                MapCellManager.SetFacePlacementPct(face.Key,face.Value);
            }
        }

        public override int GetPeopleInRange()
        {
            int pop = 0;
            var faces = MapCellManager.GetFacesAroundPoint(_areaView.Center,4);
            foreach (var face in faces)
            {
                int cellPop = (int)(GeometryUtils.CalculateCirclePolygonOverlapPct(
                    _areaView.Center, 
                    _areaView.Range, 
                    MapCellManager.GetFacePoints(face)) * (float)MapCellManager.GetPopulationInFace(face));
                
                //Debug.Log($"Face at {_mapCellManager.GetCell(face).transform.position}, is {face.data.dailyPopulation} and has {cellPop} in view of Camera");
                pop += cellPop;
            }
            return pop;
        } 
        
        public override Dictionary<HEFace, float> GetFacesInRange()
        {
            int pop = 0;
            Dictionary<HEFace, float> facesPct = new Dictionary<HEFace, float>();
            var faces = MapCellManager.GetFacesAroundPoint(_areaView.Center,4).ToList();
            foreach (var face in faces)
            {
                facesPct[face] = (GeometryUtils.CalculateCirclePolygonOverlapPct(
                    _areaView.Center, 
                    _areaView.Range, 
                    MapCellManager.GetFacePoints(face)));
            }
            return facesPct; 
        } 
        
        
        public override void Place(Vector2 pos)
        {
            StartCoroutine(nameof(SetPosition));
            base.Place(pos);
        }

        public override void Destroy()
        {
            vert?.RemoveSource();
            base.Destroy();
        }

        public override int GetRawResourceRate()
        {
            float pop = 0;
            var faces = MapCellManager.GetFacesAroundPoint(transform.position,4);
            foreach (var face in faces)
            {
                float cellPop = (int)(GeometryUtils.CalculateCirclePolygonOverlapPct(
                    _areaView.Center, 
                    _areaView.Range, 
                    MapCellManager.GetFacePoints(face)) * (float)MapCellManager.GetPopulationInFace(face) * MapCellManager.GetResourceRatioInFace(face, resource));
                
                pop += cellPop;
            }
            return (int)pop;
        }
        
        
        public override SourceDTO CaptureState()
        {
            var x = base.CaptureState();
            return new SourceDTO.MicDTO(x) {
                CenterPos = _areaView.Center
            };
        }
        
        public override async Task LoadState(SourceDTO sourceDto)
        {
            _micCount++;
            base.LoadState(sourceDto);
            
            _areaView = GetComponent<SeparatedAreaView>();
            if (sourceDto is not SourceDTO.MicDTO micDto)
            {
                return;
            }
            
            vert = MapCellManager.GetVertexClosetTo(transform.position, 2);
            vert.SetSource(this);
            
            UpdateCenterPosition(micDto.CenterPos, GetComponent<SeparatedAreaViewRenderer>());
            base.Place(transform.position);
            MapCellManager.SetMapMode(MapMode.Normal);
        }
    }
}