using UnityEngine;

namespace JSM.Surveillance.Game
{
    
    
    [RequireComponent(typeof(SeparatedAreaView))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(LineRenderer))]
    public class SeparatedAreaViewRenderer : MonoBehaviour
    {
        private SeparatedAreaView _view;
        private MeshFilter _meshFilter;

        [SerializeField] private GameObject listenerCenterPrefab;

        private GameObject listnerCenterObj = null;
        private LineRenderer _lineRenderer;
        
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _view = GetComponent<SeparatedAreaView>();
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            _meshFilter.mesh = _view.GetMesh();
            listnerCenterObj = Instantiate(listenerCenterPrefab);
            _lineRenderer.positionCount = 2;
        }

        public void RefreshMesh(Vector3 center)
        {
            if(center.Equals(Vector3.negativeInfinity))return;
            
            listnerCenterObj.transform.position = new Vector3(center.x, center.y,transform.position.z);
            _meshFilter.mesh = _view.GetMesh();
            
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, listnerCenterObj.transform.position);
        }
    }
}