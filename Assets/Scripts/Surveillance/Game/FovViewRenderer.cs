using UnityEngine;

namespace  JSM.Surveillance.Game
{
    [RequireComponent(typeof(FovView))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class FovViewRenderer : MonoBehaviour
    {
        private FovView _view;
        private MeshFilter _meshFilter;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _view = GetComponent<FovView>();
        }

        private void Start()
        {
            _meshFilter.mesh = _view.GetFOVMesh();
        }
    }
}
