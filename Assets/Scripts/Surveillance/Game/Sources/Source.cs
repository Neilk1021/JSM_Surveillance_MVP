using System;
using JSM.Surveillance.UI;
using Surveillance.Game;
using UnityEngine;

namespace JSM.Surveillance.Game
{
    public abstract class Source : MonoBehaviour
    {
        private protected MapCellManager _mapCellManager;
        private protected bool _placed = false;

        [SerializeField] private SourceUI sourceUI;
        private GameObject sourceUIObj;
        
        
        public virtual void Init(MapCellManager manager)
        {
            _mapCellManager = manager;
            _placed = false;
        }

        private void Update()
        {
            MoveSource();
            CheckIfPlaced();
        }

        public virtual void CheckIfPlaced()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Place(transform.position);
            }
        }

        protected virtual void MoveSource()
        {
            if(_placed) return;

            Vector3 currentPos = _mapCellManager.GetMouseCurrentPosition();
            if(currentPos.Equals(Vector3.negativeInfinity)) return;
            
            transform.position = new  Vector3(currentPos.x, currentPos.y, transform.position.z);
        }

        public virtual void Place(Vector2 pos)
        {
            _placed = true;
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }

        private void SwitchUIPreview()
        {
            if (!_placed) {
                return;
            }

            if (sourceUIObj == null)
            {
                sourceUIObj = Instantiate(
                    sourceUI.gameObject, 
                    transform.position,
                    Quaternion.identity
                    );
                sourceUIObj.GetComponent<SourceUI>().Init();
                return;
            }
            
            Destroy(sourceUIObj);
            sourceUIObj = null;

        }
        
        private void OnMouseDown()
        {
            SwitchUIPreview();
        }
    }
}