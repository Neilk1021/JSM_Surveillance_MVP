using System.Collections.Generic;
using JSM.Surveillance.Surveillance;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance.UI
{
    #if UNITY_EDITOR
    [ExecuteAlways]
    #endif
 
    public class MapCellRendering : MonoBehaviour
    {
        [HideInInspector] [SerializeField]
        private  MeshRenderer _meshRenderer;
        
        [FormerlySerializedAs("_lit")] [SerializeField]
        private Material lit;
        
        [FormerlySerializedAs("_face")] [SerializeField]
        private HEFace face;

        
        [HideInInspector] [SerializeField] private List<Vector2> vertices;
        [HideInInspector] [SerializeField] private Vector4[] points;
        [HideInInspector] [SerializeField] private float maxDist;
        [HideInInspector] [SerializeField] private float gradientPower;
        
        
        
        private static readonly int PointsCount = Shader.PropertyToID("_PointsCount");
        private static readonly int Points = Shader.PropertyToID("_Points");
        private static readonly int MaxDist = Shader.PropertyToID("_MaxDist");
        private static readonly int GradientPower = Shader.PropertyToID("_GradientPower");
        private static readonly int CenterColor = Shader.PropertyToID("_CenterColor");

        public void SetFace(HEFace newFace, Material newLit)
        {
            face = newFace;
            lit = newLit;
            
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material = lit;
        }

        public void SetShaderInfo(List<Vector2> vertices, Vector4[] points, float maxDist, float gradientPower)
        {
            this.vertices = vertices;
            this.points = points;
            this.maxDist = maxDist;
            this.gradientPower = gradientPower;
        }

        private void ReloadShaders()
        {
            if (vertices == null || points == null) {
                return;
            }
            
            lit.SetInt(PointsCount, vertices.Count);
            lit.SetVectorArray(Points, points);
            lit.SetFloat(MaxDist, maxDist);
            lit.SetFloat(GradientPower,0.01f);
        }
        
        #if UNITY_EDITOR
        private void OnEnable()
        {
            ReloadShaders();
        }
        #endif
        
        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material = lit;
            ReloadShaders();
            face.EnsureSOFromJson();
        }

        private void OnMouseEnter()
        {
            _meshRenderer.material.SetColor(CenterColor,new Color(0.02f,0.12f,0.2f,1));
        }

        private void OnMouseExit()
        {
            _meshRenderer.material.SetColor(CenterColor,new Color(0, 0, 0.02f, 1));
        }
    }
}