using UnityEngine;

namespace JSM.Surveillance
{
    public partial class Draggable : CellOccupier
    {
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            
            float objWidth = transform.lossyScale.x;
            float objHeight = transform.lossyScale.y; 
                    
            for (int i = 0; i < width; i++)
            {
                Vector2 hStart = new Vector2(
                    transform.position.x  - objWidth/2 + (objWidth/width)*i,
                    transform.position.y  - objHeight/2
                );
                Vector2 hEnd = new Vector2(
                    transform.position.x  - objWidth/2 + (objWidth/width)*i,
                    transform.position.y  + objHeight/2
                );
                
                Gizmos.DrawLine(hStart, hEnd);
            }
            
            
            for (int j = 0; j <  height; j++)
            {
        
                Vector2 vStart = new Vector2(
                    transform.position.x - objWidth/2,
                    transform.position.y - objHeight/2 + (objHeight/height) * j
                );
                Vector2 vEnd = new Vector2(
                    transform.position.x + objWidth/2,
                    transform.position.y - objHeight/2 + (objHeight/height) * j
                );
                    
                    
                Gizmos.DrawLine(vStart, vEnd);
            }
        }
    }
}