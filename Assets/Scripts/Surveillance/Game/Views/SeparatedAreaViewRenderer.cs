using UnityEngine;

namespace JSM.Surveillance.Game
{
    
    
    [RequireComponent(typeof(SeparatedAreaView))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class SeparatedAreaViewRenderer : MonoBehaviour
    {
        private SeparatedAreaView _view;
        private MeshFilter _meshFilter;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _view = GetComponent<SeparatedAreaView>();
        }

        private void Start()
        {
            _meshFilter.mesh = _view.GetMesh();
        }

        public void RefreshMesh()
        {
            _meshFilter.mesh = _view.GetMesh();
        }
    }
}