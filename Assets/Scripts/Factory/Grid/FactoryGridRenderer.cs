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
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tile defaultTile;
        
        
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

        public void UpdateCellVisual(int x, int y)
        {
            var cellData = _grid.GetCell(x, y);
            tilemap.SetTile(new Vector3Int(x,y,0), defaultTile);
        }
    }
}
