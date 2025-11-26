using System;
using System.Collections;
using System.Threading.Tasks;
using JSM.Surveillance.Util;
using UnityEngine;

namespace JSM.Surveillance.Game
{
    [RequireComponent(typeof(FovView))]
    public class CameraSource : Source
    {
        private MapEdgeVertex vert = null;
        private FovView _fovView;

        private void Start()
        {
            _fovView = GetComponent<FovView>();
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
                Place(transform.position);
            }
        }


        IEnumerator SetRotation()
        {
            Vector3 initialMousePosition = Input.mousePosition;
            float initialZRotation = transform.rotation.eulerAngles.z;
            
            yield return new WaitForFixedUpdate();
            
            while (true)
            {
                
                Vector2 delta = (Input.mousePosition - initialMousePosition);
                transform.rotation = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,
                    transform.rotation.eulerAngles.y,
                    initialZRotation + (delta.y - delta.x)  / 2);

                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }

                yield return null;
            }
        }
        
        public override int GetPeopleInRange(float radius = 2)
        {
            int pop = 0;
            var faces = _mapCellManager.GetFacesAroundPoint(transform.position,5);
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
        
        public override void Place(Vector2 pos)
        {
            StartCoroutine(nameof(SetRotation));
            base.Place(pos);
        }
    }
}