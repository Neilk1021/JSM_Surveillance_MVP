using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        
        public override void Init(MapCellManager manager, SourceData data)
        {
            base.Init(manager, data);
            _micCount++;
            sourceName += $" {_micCount}";
        }
        
        protected override void MoveSource()
        {
            if(_placed) return;

            var mousePos = _mapCellManager.GetMouseCurrentPosition();
            vert = _mapCellManager.GetVertexClosetTo(mousePos, 0.5f);
            
            Vector3 currentPos;
            
            if (vert == null) {
                currentPos = mousePos;
            }
            else { 
                currentPos = vert.transform.position;
            }
            
            transform.position = new  Vector3(currentPos.x, currentPos.y, transform.position.z);
        }

        public override void CheckIfPlaced()
        {
            if (!_placed && Input.GetMouseButtonDown(0) && vert is not null)
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

            _mapCellManager.SetMapMode(MapMode.Placement);
            var renderer = GetComponent<SeparatedAreaViewRenderer>();
            yield return new WaitForFixedUpdate();
            
            var lastMousePos =  _mapCellManager.GetMouseCurrentPosition();
            while (true)
            {
                var mousePos = _mapCellManager.GetMouseCurrentPosition();
                renderer.RefreshMesh(mousePos);
                if ((mousePos - lastMousePos).magnitude > 0.1f)
                {
                    lastMousePos = mousePos;
                    _areaView.SetCenter(mousePos);

                    float rad = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
                    micModel.rotation = Quaternion.Euler(micModel.eulerAngles.x, micModel.eulerAngles.y, -rad * Mathf.Rad2Deg-90);
                    
                    
                    var faces = GetFacesInRange();
                    foreach (var face in faces)
                    {
                        _mapCellManager.SetFacePlacementPct(face.Key,face.Value);
                    }
                }
                
                
                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }

                yield return null;
            }

            _mapCellManager.SetMapMode(MapMode.Normal);
        }
        
        public override int GetPeopleInRange(float radius = 2)
        {
            int pop = 0;
            var faces = _mapCellManager.GetFacesAroundPoint(transform.position,4);
            foreach (var face in faces)
            {
                int cellPop = (int)(GeometryUtils.CalculateCirclePolygonOverlapPct(
                    _areaView.Center, 
                    _areaView.Range, 
                    _mapCellManager.GetFacePoints(face)) * (float)_mapCellManager.GetPopulationInFace(face));
                
                //Debug.Log($"Face at {_mapCellManager.GetCell(face).transform.position}, is {face.data.dailyPopulation} and has {cellPop} in view of Camera");
                pop += cellPop;
            }
            return pop;
        } 
        
        public override Dictionary<HEFace, float> GetFacesInRange(float radius = 2)
        {
            int pop = 0;
            Dictionary<HEFace, float> facesPct = new Dictionary<HEFace, float>();
            var faces = _mapCellManager.GetFacesAroundPoint(transform.position,4).ToList();
            foreach (var face in faces)
            {
                facesPct[face] = (GeometryUtils.CalculateCirclePolygonOverlapPct(
                    _areaView.Center, 
                    _areaView.Range, 
                    _mapCellManager.GetFacePoints(face)));
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
    }
}