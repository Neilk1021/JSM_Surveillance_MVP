using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JSM.Surveillance
{
    [System.Serializable]
    public struct BorderRadius
    {
        public float left, right, up, down;
    }
    
    [RequireComponent(typeof(FactoryGrid))]
    public class FactoryGridRenderer : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private FactoryGrid _grid;
        
        [Header("Tile Map")]
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tile defaultTile;

        [Header("Resizing Constraints")] [SerializeField]
        private BorderRadius borderRadius; 

        [Header("Backgrounds")] [SerializeField]
        private SpriteRenderer foreground;
        
        [Header("Canvas")] [SerializeField]
        [Range(0, 2)] private float headerSize;

        [SerializeField] [Range(0, 2)] private float shopSize;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform ShopUI;
        [SerializeField] private RectTransform HeaderUI;
        
        private const float CanvasScale = 0.01f;

        [SerializeField] private FactoryGridBackgroundFader backgroundFader;
        
        public void Awake() {
            _grid = GetComponent<FactoryGrid>();
            tilemap ??= GetComponent<Tilemap>();


            var fader = Instantiate(backgroundFader, transform.parent);
            _grid.OnGridClose.AddListener(fader.Close);
            _grid.OnMouseEnter.AddListener(fader.ShowFade);
            _grid.OnMouseExit.AddListener(fader.HideFade);
        }

        private void Start()
        {
            DrawGrid();
        }

        [ContextMenu("Draw Grid")]
        private void DrawGrid()
        {
            _grid ??= GetComponent<FactoryGrid>();
            tilemap ??= GetComponent<Tilemap>();
            
            for (var x = 0; x < _grid.Width; x++) {
                for (var y = 0; y < _grid.Height; y++) {
                    UpdateCellVisual(x,y);
                }
            }
        }

        [ContextMenu("Clear Grid")]
        public void ClearGrid()
        {
            tilemap ??= GetComponent<Tilemap>();
            tilemap.ClearAllTiles();
        }

        [ContextMenu("Regenerate Sizing")]
        public void ResizeBackground()
        {
            _grid ??= GetComponent<FactoryGrid>();
            var fTransform = foreground.transform;
            var size = new Vector3(_grid.Width, _grid.Height) * _grid.CellSize;
            
            foreground.size = size;
            
            fTransform.localPosition = size / 2;
            var bc = GetComponent<BoxCollider>();
            
            bc.size = (Vector3)size + new Vector3(borderRadius.right + borderRadius.left, borderRadius.up + borderRadius.down);
            bc.center = (Vector3)size/2 + new Vector3(borderRadius.right - borderRadius.left,borderRadius.up - borderRadius.down,0)/2;

            ResizeCanvas(size);

        }

        private void ResizeCanvas(Vector3 size)
        {
            _grid ??= GetComponent<FactoryGrid>();
            var rt = canvas.GetComponent<RectTransform>();

            rt.localScale = new Vector3(CanvasScale, CanvasScale, CanvasScale);
            
            var gridSize = (size + new Vector3(borderRadius.right + borderRadius.left, borderRadius.up + borderRadius.down)) /  CanvasScale;

            var rightDelta = shopSize;
            var topDelta = headerSize;

            Vector3 upRightDelta = new Vector2(shopSize, headerSize);
            Vector3 resizedDelta = upRightDelta / CanvasScale;

            var finalScale = gridSize + resizedDelta;

            rt.sizeDelta = finalScale; 
            rt.localPosition = (size + upRightDelta) / 2 + new Vector3(borderRadius.right - borderRadius.left,borderRadius.up - borderRadius.down,0)/2;;

            ShopUI.sizeDelta = new Vector2(shopSize / CanvasScale, finalScale.y);
            //TODO ACCOUNT FOR pivot position
            ShopUI.anchoredPosition = Vector3.zero;
            
            HeaderUI.sizeDelta = new Vector2(gridSize.x, headerSize / CanvasScale + gridSize.y);
            //TODO ACCOUNT FOR pivot position!
            HeaderUI.anchoredPosition = Vector3.zero - new Vector3(resizedDelta.x / 2, 0, 0);


        }

        public void UpdateCellVisual(int x, int y)
        {
            tilemap.SetTile(new Vector3Int(x,y,0), defaultTile);
        }
    }
}
