using System.Collections.Generic;
using JSM.Surveillance.Game;
using JSM.Surveillance.Surveillance;
using Surveillance.Game;
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

        private MapMode _renderingMode = MapMode.Normal;
        
        [HideInInspector] [SerializeField] private List<Vector2> vertices;
        [HideInInspector] [SerializeField] private Vector4[] points;
        [HideInInspector] [SerializeField] private float maxDist;
        [HideInInspector] [SerializeField] private float gradientPower;
        [HideInInspector] [SerializeField] private Color defaultInteriorColor;
        [HideInInspector] [SerializeField] private Color defaultExteriorColor;
        [HideInInspector] [SerializeField] private Color defaultBorderColor;
        
        private static readonly int PointsCount = Shader.PropertyToID("_PointsCount");
        private static readonly int Points = Shader.PropertyToID("_Points");
        private static readonly int MaxDist = Shader.PropertyToID("_MaxDist");
        private static readonly int GradientPower = Shader.PropertyToID("_GradientPower");
        private static readonly int CenterColor = Shader.PropertyToID("_CenterColor");
        
        private static readonly int BorderColor = Shader.PropertyToID("_BorderColor");
        private static readonly int EdgeColor = Shader.PropertyToID("_EdgeColor");
        private MapCell _cell;
        public MapCell Cell => _cell;
        

        public void SetFace(HEFace newFace, Material newLit)
        {
            face = newFace;
            lit = newLit;
            
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material = lit;
            defaultInteriorColor = lit.GetColor(CenterColor);
            defaultExteriorColor = lit.GetColor(EdgeColor);
            defaultBorderColor = lit.GetColor(BorderColor);
        }

        public void SetShaderInfo(List<Vector2> vertices, Vector4[] points, float maxDist, float gradientPower)
        {
            this.vertices = vertices;
            this.points = points;
            this.maxDist = maxDist;
            this.gradientPower = gradientPower;
        }

        public void ResetColor()
        {
            _meshRenderer.material = lit;
            lit.SetColor(CenterColor, defaultInteriorColor);
            lit.color = defaultExteriorColor;
            lit.SetColor(BorderColor,defaultBorderColor);
            
            lit.SetColor(EdgeColor, defaultExteriorColor);
        }
        
        public void SetColor(Color interiorColor, Color exteriorColor)
        {
            _meshRenderer.material = lit;
            lit.SetColor(CenterColor,  interiorColor);
            lit.SetColor(EdgeColor, exteriorColor);
            lit.color = exteriorColor;
        }

        public void SetBorderColor(Color color)
        {
            lit.SetColor(BorderColor, color);
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

        public void SetMode(MapMode mode)
        {
            if (mode == MapMode.Placement)
            {
                _meshRenderer.material.SetColor(CenterColor, new Color(0, 0, 0, 1));
            }

            if (mode == MapMode.Normal)
            {
                _meshRenderer.material.SetColor(CenterColor,new Color(0, 0, 0.02f, 1)); 
            }
            
            _renderingMode = mode;
        }
        
        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _cell = GetComponent<MapCell>();
            _meshRenderer.material = lit;
        }

        private void Start()
        {
            ReloadShaders();
            face.EnsureSOFromJson();
        }

        private void OnMouseEnter()
        {
            if (_renderingMode != MapMode.Normal)
            {
                return;
            }
            
            _meshRenderer.material.SetColor(CenterColor,new Color(0.02f,0.12f,0.2f,1));
        }

        private void OnMouseExit()
        {
            if (_renderingMode != MapMode.Normal)
            {
                return;
            }
            
            _meshRenderer.material.SetColor(CenterColor,new Color(0, 0, 0.02f, 1));
        }

        public void SetAlpha(float faceValue)
        {
            if(!_cell.IsStreet)return;
            
            Color color = Color.Lerp(Color.black, Color.green, faceValue);
            
            _meshRenderer.material.SetColor(CenterColor, color);
        }
    }
}