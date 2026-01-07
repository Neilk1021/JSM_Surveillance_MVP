using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JSM.Surveillance.Surveillance;
using JSM.Surveillance.UI;
using JSM.Surveillance.Util;
using Surveillance.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace JSM.Surveillance.Game
{
    public abstract partial class Source : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] protected string sourceName;
        [SerializeField] private int maxIncomingSourceLinks = 0;
        [SerializeField] private LinkRenderer linkRendererPrefab;
        [SerializeField] private SourceUI sourceUI;

        private LinkRenderer _linkRenderer;
        private protected MapCellManager MapCellManager;
        private protected bool Placed = false;
        private SourceData _data;
        private Source _nextSource = null;
        private Source[] _incomingSources;
        
        public SourceData Data => _data;
        public string SourceName => sourceName;
        public int MaxIncomingSourceLinks => maxIncomingSourceLinks;
        public Source[] IncomingSourceLinks => _incomingSources;
        public Source NextSource => _nextSource;
        
        public readonly UnityEvent OnModified = new UnityEvent();
        public readonly UnityEvent OnIncomingSourcesChanged = new UnityEvent();
        
        
        public virtual void Init(MapCellManager manager, SourceData data)
        {
            _incomingSources = new Source[maxIncomingSourceLinks];
            sourceName = data.ShopInfo.name;
            _data = data;
            MapCellManager = manager;
            Placed = false;
            OnModified.AddListener(ReloadNextSource);
        }

        private void Update()
        {
            MoveSource();
            CheckIfPlaced();
        }

        protected virtual void CheckIfPlaced()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Place(transform.position);
            }
        }

        public bool LinkToSource(Source dest)
        {
            if (!dest.AddIncomingSource(this)) return false;
            
            if (_nextSource != null)
            {
                _nextSource.RemoveIncomingSource(this);
            }
            
            _nextSource = dest;
            return true;
        }

        private bool RemoveIncomingSource(Source source)
        {
            for (int i = 0; i < _incomingSources.Length; i++)
            {
                if (_incomingSources[i] != source) continue;
                
                _incomingSources[i] = null;
                return true;
            }

            return false;
        }

        private bool AddIncomingSource(Source source)
        {
            if (_incomingSources.Contains(source)) return false;
            
            for (int i = 0; i < _incomingSources.Length; i++)
            {
                if (_incomingSources[i] != null) continue;
                _incomingSources[i] = source;
                OnIncomingSourcesChanged?.Invoke();
                return true;
            }

            return false;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected virtual void MoveSource()
        {
            if(Placed) return;

            Vector3 currentPos = MapCellManager.GetMouseCurrentPosition();
            if(currentPos.Equals(Vector3.negativeInfinity)) return;
            
            transform.position = new  Vector3(currentPos.x, currentPos.y, transform.position.z);
        }

        public virtual void Place(Vector2 pos)
        {
            Placed = true;
    
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }

        public SourceUI CreateUI()
        {
            if (!Placed) {
                return null;
            }
            
            var sourceUIObj = Instantiate(
                sourceUI.gameObject, 
                transform.position,
                Quaternion.identity
            );
            var sourceUIComponent = sourceUIObj.GetComponent<SourceUI>();
            sourceUIComponent.Init(this, MapCellManager); 
            
            return sourceUIComponent;
        }

        public abstract int GetPeopleInRange();
        public abstract Dictionary<HEFace, float> GetFacesInRange();
        
        
        private void OnMouseDown()
        {
            MapCellManager.SwitchUIPreview(this);
        }

        public void CloseUI()
        {
            MapCellManager.CloseUIPreview();
        }

        public virtual void Destroy()
        {
            if (_nextSource != null)
            {
                _nextSource.RemoveIncomingSource(this);
            }

            foreach (var incomingSource in _incomingSources)
            {
                if (incomingSource != null)
                {
                    incomingSource.ClearNextSource();
                }
            }
            
            if(_grid != null) Destroy(_grid.gameObject);
            Destroy(gameObject);
        }

        private void ClearNextSource()
        {
            _nextSource = null;
            if(_linkRenderer)
                Destroy(_linkRenderer.gameObject);
        }

        public void UpdateName(string newName)
        {
            sourceName = newName;
        }

        public void StartLinking()
        {
            if (_linkRenderer != null)
            {
                Destroy(_linkRenderer.gameObject);
            }

            StartCoroutine(Link());
        }

        private IEnumerator Link()
        {
            _linkRenderer = Instantiate(linkRendererPrefab, transform);
            _linkRenderer.SetStart(transform.position);
            Source destSource = null;
            while (true)
            {
                yield return null;

                var mousePos = MapCellManager.GetMouseCurrentPosition();
                if(mousePos == Vector3.negativeInfinity) continue;
                
                destSource = SurveillanceGameManager.instance.GetSourceClosetTo(mousePos, 0.5f);
                if (destSource == this) destSource = null;
                
                var currentPos = !destSource ? mousePos : destSource.transform.position;
                _linkRenderer.SetEnd(currentPos);
                
                
                if (Input.GetMouseButtonDown(0))
                {
                    break;
                }
            }
            
            if (!destSource)
            {
                if (_nextSource)
                {
                    _nextSource.RemoveIncomingSource(this);
                }

                _nextSource = null;
                if(_linkRenderer)
                    Destroy(_linkRenderer.gameObject);
                
                OnModified?.Invoke();
               yield break; 
            }

            if (LinkToSource(destSource))
            {
                _linkRenderer.SetEnd(destSource.transform.position);
                OnModified?.Invoke();
                yield break;
            }
            Destroy(_linkRenderer.gameObject);
        }

        public abstract int GetRawResourceRate();

        private void ReloadNextSource()
        {
            _nextSource?.Modified();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
           OnMouseDown(); 
        }
    }
}