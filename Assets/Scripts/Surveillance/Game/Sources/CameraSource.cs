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
    [RequireComponent(typeof(FovView))]
    public class CameraSource : Source
    {
        private MapEdgeVertex vert = null;
        private FovView _fovView;
        private static int _cameraCount = 0; 

        private void Start()
        {
            _fovView = GetComponent<FovView>();
        }


        public override void Init(MapCellManager manager, SourceData data)
        {
            base.Init(manager, data);
            _cameraCount++;
            sourceName += $" {_cameraCount}";
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

        protected override void CheckIfPlaced()
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


        IEnumerator SetRotation()
        {
            Vector3 initialMousePosition = Input.mousePosition;
            float initialZRotation = transform.rotation.eulerAngles.z;

            _mapCellManager.SetMapMode(MapMode.Placement);
            yield return new WaitForFixedUpdate();
            
            while (true)
            {
                
                Vector2 delta = (Input.mousePosition - initialMousePosition);
                
                Quaternion newRot = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,
                    transform.rotation.eulerAngles.y,
                    initialZRotation + (delta.y - delta.x)  / 2);

                if (Quaternion.Angle(transform.rotation, newRot) > 0.1f)
                {
                    var faces = GetFacesInRange();
                    foreach (var face in faces)
                    {
                        _mapCellManager.SetFacePlacementPct(face.Key,face.Value);
                    }
                    transform.rotation = newRot;
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

                int cellPop = (int)(GeometryUtils.CalculateSectorPolygonOverlapPct(
                    transform.position, 
                    _fovView.Range, 
                    _mapCellManager.GetFacePoints(face),_fovView.GetDirection() , 
                    _fovView.FOV) * (float)_mapCellManager.GetPopulationInFace(face));
                
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
                facesPct[face] = (GeometryUtils.CalculateSectorPolygonOverlapPct(
                    transform.position, 
                    _fovView.Range, 
                    _mapCellManager.GetFacePoints(face),_fovView.GetDirection() , 
                    _fovView.FOV));
            }
            return facesPct; 
        } 

        
        
        public override void Place(Vector2 pos)
        {
            StartCoroutine(nameof(SetRotation));
            base.Place(pos);
        }

        public override void Destroy()
        {
            vert?.RemoveSource();
            base.Destroy();
        }
    }
}