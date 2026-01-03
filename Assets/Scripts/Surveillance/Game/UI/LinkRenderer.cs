using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class LinkRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lr;

        public void SetEnd(Vector3 currentPos)
        {
            lr.SetPosition(1, currentPos);
            
        }

        public void SetStart(Vector3 transformPosition)
        {
            lr.SetPosition(0,transformPosition);
        }
    }
}