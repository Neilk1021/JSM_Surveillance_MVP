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
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Color validColor, invalidColor;
        
        public override Vector2Int Size => new Vector2Int(width, height);

      
        protected bool Placed = false;
        protected SortingGroup SortingGroup;

        protected virtual void Awake()
        {
            SortingGroup = GetComponent<SortingGroup>();
        }

        protected virtual void OnMouseDown()
        {
        }

        public override void Place(List<Vector2Int> newPositions, Vector3 worldPos, FactoryGrid grid)
        {
            base.Place(newPositions, worldPos, grid);
            Placed = true;
        }

        public virtual void Rotate()
        {
            Rotation += 90;
            Rotation %= 360;

            transform.eulerAngles += new Vector3(0, 0, 90);
        }
 
        // private void Update()
        // {
        //     if (!_isDragging || Placed) return;
        //
        //     Vector2 mousePos = _camera.ScreenToWorldPoint(End.mousePosition);
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
            SortingGroup.sortingOrder += SurveillanceWindow.GlobalSortOrder;
            StartCoroutine(FollowMouseUntilPlace());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected virtual IEnumerator FollowMouseUntilPlace()
        {
            Color startingColor = spriteRenderer.color;
            yield return null;
            SortingGroup.sortAtRoot = true;
            while (!Placed)
            {
                yield return new WaitUntil(() =>
                {
                    if (Grid.MouseOverGrid())
                    {
                        spriteRenderer.color =  (Grid.IsCellOccupierValid(this) ? validColor : invalidColor);
                        var gridPos = Grid.GetGridPosition(Grid.GetMouseWorldPos3D());

                        Vector2Int rotatedSize = Rotation % 180 == 0 ? Size : new Vector2Int(Size.y, Size.x);
                        
                        Vector3 offset = new Vector2(
                            rotatedSize.x  % 2 == 0 ? 0 : Grid.CellSize / 2.0f,
                            rotatedSize.y % 2 == 0 ? 0 : Grid.CellSize / 2.0f
                            );
                        
                        transform.position = Grid.GetWorldPosition(gridPos) + offset;
                        
                    }
                    else
                    {
                        spriteRenderer.color = invalidColor;
                        Vector2 mousePos = Grid.GetMouseWorldPos2D();
                        transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
                    }

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        Rotate();
                    }

                    //TODO allow cancels
                    return Input.GetMouseButtonDown(0);
                });
                Placed = Grid.PlaceCellOccupierAtCurrentPosition(this);

                
                yield return new WaitForEndOfFrame();
            }

            spriteRenderer.color = startingColor;
            SortingGroup.sortAtRoot = false;
        }
    }
}