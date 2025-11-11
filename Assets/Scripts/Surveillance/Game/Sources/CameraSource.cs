using System;
using UnityEngine;

namespace JSM.Surveillance.Game
{
    public class CameraSource : Source
    {
        private MapEdgeVertex edge = null;
        
        protected override void MoveSource()
        {
            if(_placed) return;

            var mousePos = _mapCellManager.GetMouseCurrentPosition();
            edge = _mapCellManager.GetVertexClosetTo(mousePos, 0.5f);
            
            Vector3 currentPos;
            
            if (edge == null) {
                currentPos = mousePos;
            }
            else { 
                currentPos = edge.transform.position;
            }
            
            transform.position = new  Vector3(currentPos.x, currentPos.y, transform.position.z);
        }

        public override void CheckIfPlaced()
        {
            if (Input.GetMouseButtonDown(0) && edge is not null)
            {
                Place(transform.position);
            }
        }
        
    }
}