using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JSM.Surveillance
{
    [RequireComponent(typeof(FactoryGrid))]
    public class FactoryGridRenderer : MonoBehaviour
    {
        private FactoryGrid _grid;
        
        [Header("Tile Map")]
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tile defaultTile;

        [Header("Resizing Constraitns")] [SerializeField]
        private float borderRadius;
        
        [Header("Backgrounds")] [SerializeField]
        private SpriteRenderer background;

        [Header("Canvas")] [SerializeField]
        [Range(0, 2)] private float headerSize;

        [SerializeField] [Range(0, 2)] private float shopSize;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform ShopUI;
        [SerializeField] private RectTransform HeaderUI;
        
        private const float CanvasScale = 0.01f;
        
        
        public void Awake() {
            _grid = GetComponent<FactoryGrid>();
            tilemap ??= GetComponent<Tilemap>();
        }

        private void Start()
        {
            DrawGrid();
        }

        private void DrawGrid()
        {
            for (var x = 0; x < _grid.Width; x++) {
                for (var y = 0; y < _grid.Height; y++) {
                    UpdateCellVisual(x,y);
                }
            }
        }

        [ContextMenu("Regenerate Sizing")]
        public void ResizeBackground()
        {
            _grid ??= GetComponent<FactoryGrid>();
            var bTransform = background.transform;
            var size = new Vector3(_grid.Width, _grid.Height) * _grid.CellSize;
            bTransform.localScale =
                size
                + new Vector3(borderRadius, borderRadius);

            bTransform.localPosition = size / 2;
            var bc = GetComponent<BoxCollider>();
            
            bc.size = size + new Vector3(borderRadius, borderRadius, borderRadius);
            bc.center = size / 2;

            ResizeCanvas(size);

        }

        private void ResizeCanvas(Vector3 size)
        {
            _grid ??= GetComponent<FactoryGrid>();
            var rt = canvas.GetComponent<RectTransform>();

            rt.localScale = new Vector3(CanvasScale, CanvasScale, CanvasScale);
            
            var gridSize = (size + new Vector3(borderRadius, borderRadius)) /  CanvasScale;

            var rightDelta = shopSize;
            var topDelta = headerSize;

            Vector3 upRightDelta = new Vector2(shopSize, headerSize);
            Vector3 resizedDelta = upRightDelta / CanvasScale;

            var finalScale = gridSize + resizedDelta;

            rt.sizeDelta = finalScale; 
            rt.localPosition = (size + upRightDelta) / 2;

            ShopUI.sizeDelta = new Vector2(shopSize / CanvasScale, finalScale.y);
            //TODO ACCOUNT FOR pivot position
            ShopUI.localPosition = Vector3.zero;
            
            HeaderUI.sizeDelta = new Vector2(finalScale.x, headerSize / CanvasScale);
            //TODO ACCOUNT FOR pivot position!
            HeaderUI.localPosition = Vector3.zero;


        }

        public void UpdateCellVisual(int x, int y)
        {
            var cellData = _grid.GetCell(x, y);
            tilemap.SetTile(new Vector3Int(x,y,0), defaultTile);
        }
    }
}
