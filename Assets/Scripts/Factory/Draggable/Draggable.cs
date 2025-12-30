using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.UI;
using UnityEngine;
using UnityEngine.Rendering;

namespace JSM.Surveillance
{
    public partial class Draggable : CellOccupier 
    {
        [Range(1,10)]
        [SerializeField] private int width, height;
        [Header("Preview Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color validColor, invalidColor;
        
        public override Vector2Int Size => new Vector2Int(width, height);
        
        protected bool Placed = false;
        private SortingGroup _sortingGroup;

        protected virtual void Awake()
        {
            _sortingGroup = GetComponent<SortingGroup>();
        }

        protected virtual void OnMouseDown()
        {
        }

        public override void Place(List<Vector2Int> newPositions, Vector3 worldPos, FactoryGrid grid)
        {
            base.Place(newPositions, worldPos, grid);
            Placed = true;
        }

        // private void Update()
        // {
        //     if (!_isDragging || Placed) return;
        //
        //     Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        //     transform.position = mousePos;
        // }

        //TODO implement (this is you haiyi.)
        public override void Entered()
        {
           // haiyi this is functionally the same as OnMouseEntered, but its based on the cell in the grid.
        }

        public override void Exited()
        {
            // functionally the same as OnMouseExited, but its based on the cell in the grid. 
        }

        
        public void StartPlacement()
        {
            StopAllCoroutines();
            Placed = false;
            _sortingGroup.sortingOrder += SurveillanceWindow.GlobalSortOrder;
            StartCoroutine(FollowMouseUntilPlace());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        IEnumerator FollowMouseUntilPlace()
        {
            Color startingColor = spriteRenderer.color;
            yield return null;
            _sortingGroup.sortAtRoot = true;
            while (!Placed)
            {
                yield return new WaitUntil(() =>
                {
                    if (Grid.MouseOverGrid())
                    {
                        spriteRenderer.color =  (Grid.IsCellOccupierValid(this) ? validColor : invalidColor);
                        var gridPos = Grid.GetGridPosition(Grid.GetMouseWorldPos3D());
                        transform.position = Grid.GetWorldPosition(gridPos);
                    }
                    else
                    {
                        spriteRenderer.color = invalidColor;
                        Vector2 mousePos = Grid.GetMouseWorldPos2D();
                        transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
                    }
                    
                    
                    return Input.GetMouseButtonDown(0);
                });
                Placed = Grid.PlaceCellOccupierAtCurrentPosition(this);
                yield return new WaitForEndOfFrame();
            }

            spriteRenderer.color = startingColor;
            _sortingGroup.sortAtRoot = false;
        }
    }
}