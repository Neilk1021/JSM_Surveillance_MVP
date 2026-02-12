using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JSM.Surveillance.Surveillance;
using JSM.Surveillance.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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


        public override void Init(MapCellManager manager, SourceData data, bool placeImmediate = false)
        {
            base.Init(manager, data);
            _cameraCount++;
            sourceName += $" {_cameraCount}";
            
            if(placeImmediate == false) MapCellManager.SetMapMode(MapMode.Population);
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


        IEnumerator SetRotation()
        {
            Vector3 initialMousePosition = Input.mousePosition;
            float initialZRotation = transform.rotation.eulerAngles.z;

            MapCellManager.SetAllFacePlacementPct(0);
            UpdatePlacementPct(transform.rotation);
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
                    UpdatePlacementPct(newRot);
                }
                
                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }

                yield return null;
            }

            MapCellManager.SetMapMode(MapMode.Normal);
        }

        private void UpdatePlacementPct(Quaternion newRot)
        {
            var faces = GetFacesInRange();
            foreach (var face in faces)
            {
                MapCellManager.SetFacePlacementPct(face.Key,face.Value);
            }
            transform.rotation = newRot;
        }

        public override int GetPeopleInRange()
        {
            int pop = 0;
            var faces = MapCellManager.GetFacesAroundPoint(transform.position,4);
            foreach (var face in faces)
            {

                int cellPop = (int)(GeometryUtils.CalculateSectorPolygonOverlapPct(
                    transform.position, 
                    _fovView.Range, 
                    MapCellManager.GetFacePoints(face),_fovView.GetDirection() , 
                    _fovView.FOV) * (float)MapCellManager.GetPopulationInFace(face));
                
                //Debug.Log($"Face at {_mapCellManager.GetCell(face).transform.position}, is {face.data.dailyPopulation} and has {cellPop} in view of Camera");
                pop += cellPop;
            }
            return pop;
        } 
        
        public override Dictionary<HEFace, float> GetFacesInRange()
        {
            int pop = 0;
            Dictionary<HEFace, float> facesPct = new Dictionary<HEFace, float>();
            var faces = MapCellManager.GetFacesAroundPoint(transform.position,4).ToList();
            foreach (var face in faces)
            {
                facesPct[face] = (GeometryUtils.CalculateSectorPolygonOverlapPct(
                    transform.position, 
                    _fovView.Range, 
                    MapCellManager.GetFacePoints(face),_fovView.GetDirection() , 
                    _fovView.FOV));
            }
            return facesPct; 
        } 

        
        
        public override void Place(Vector2 pos)
        {
            if (vert is null) return;
            if (vert.GetSource() != null) {
                return;
            }
            vert.SetSource(this);
            StartCoroutine(nameof(SetRotation));
            base.Place(pos);
        }

        public override void Destroy()
        {
            vert?.RemoveSource();
            base.Destroy();
        }

        public override int GetRawResourceRate()
        {
            return GetPeopleInRange();
        }
        
        
        public override SourceDTO CaptureState()
        {
            var x = base.CaptureState();
            return new SourceDTO.CameraDTO(x) {
                Angle = transform.rotation.eulerAngles.z
            };
        }
        
        public override async Task LoadState(SourceDTO sourceDto)
        {
            base.LoadState(sourceDto);
            
            _fovView = GetComponent<FovView>();
            if (sourceDto is not SourceDTO.CameraDTO cameraDto){
                return;
            }

            Quaternion newRot = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y,
                cameraDto.Angle);
            
            vert = MapCellManager.GetVertexClosetTo(transform.position, 3);
            vert.SetSource(this);

            base.Place(transform.position);
            transform.rotation = newRot;
        }
        
    }
}