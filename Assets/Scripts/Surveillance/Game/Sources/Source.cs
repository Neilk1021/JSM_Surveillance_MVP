using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JSM.Surveillance.Saving;
using JSM.Surveillance.Surveillance;
using JSM.Surveillance.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace JSM.Surveillance.Game
{
    public abstract partial class Source : MonoBehaviour, IPointerDownHandler
    {
        private Guid _guid;
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
        
        
        public virtual void Init(MapCellManager manager, SourceData data, bool placeImmediate = false)
        {
            _incomingSources = new Source[maxIncomingSourceLinks];
            sourceName = data.ShopInfo.name;
            _data = data;
            MapCellManager = manager;
            Placed = false;
            _guid = Guid.NewGuid();
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

        public Guid GetGuid()
        {
            return _guid;
        }

        public virtual SourceDTO CaptureState()
        {
            SimulationSaveData sim = (SimulationSaveData)_simulation?.CaptureState();
            Guid nextSourceGuid = (_nextSource != null) ? _nextSource._guid : Guid.Empty;
            List<Guid> incomingGuids = new List<Guid>();
            foreach (var source in _incomingSources)
            {
                if (source != null)
                {
                    incomingGuids.Add(source._guid);
                }
            }
            
            return new SourceDTO()
            {
                Guid = _guid,
                Simulation = sim ,
                lastLayout = _lastLayout,
                sourceDataPath = _data.AssetGuid,
                NextSource = nextSourceGuid,
                IncomingSources = incomingGuids, 
                position = transform.position,
                sourceName = SourceName
            };
        }

        public virtual async Task LoadState(SourceDTO sourceDto)
        {
            MapCellManager = FindObjectOfType<MapCellManager>();
            
            _incomingSources = new Source[maxIncomingSourceLinks];
            _guid = sourceDto.Guid;
            sourceName = sourceDto.sourceName;
            transform.position = sourceDto.position;
            
            var pData = Addressables.LoadAssetAsync<SourceData>(sourceDto.sourceDataPath);
            await pData.Task;
            
            if (pData.Status == AsyncOperationStatus.Succeeded ) {
                _data = pData.Result;
            }
            else {
                throw new ArgumentException($"Couldn't find source data at address {sourceDto.sourceDataPath}");
            }

            _lastLayout = sourceDto.lastLayout;
            
            OnModified.AddListener(ReloadNextSource);
        }

        public virtual async Task RehydrateState(SourceDTO dto, Dictionary<Guid, Source> allSources)
        {
            if (dto.NextSource != Guid.Empty && allSources.TryGetValue(dto.NextSource, out var next))
            {
                _linkRenderer = Instantiate(linkRendererPrefab, transform);
                _linkRenderer.SetStart(transform.position);
                this.LinkToSource(next);
                _linkRenderer.SetEnd(next.transform.position);
            }

            foreach (var incGuid in dto.IncomingSources)
            {
                if (allSources.TryGetValue(incGuid, out var incoming))
                {
                    this.AddIncomingSource(incoming);
                }
            }

            if (dto.Simulation != null)
            {
                dto.Simulation.RehydrateSourceReferences(allSources);
                var sim = new FactoryGridSimulation(this);
                await sim.LoadState(dto.Simulation);
        
                SetSimulation(sim);
            }
        }
    }
}