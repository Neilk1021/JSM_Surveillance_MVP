using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JSM.Surveillance.Saving;
using JSM.Surveillance.Surveillance;
using JSM.Surveillance.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif


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
        //private protected PlacementStatus Placed = PlacementStatus.AwaitingPlacement; 
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

        private bool _placed = false;
        private TaskCompletionSource<bool> _placedProcess = new TaskCompletionSource<bool>(); 
        
        [SerializeField] private LayoutObject defaultLayout;
        
        public virtual void Init(MapCellManager manager, SourceData data, bool placeImmediate = false)
        {
            _incomingSources = new Source[maxIncomingSourceLinks];
            sourceName = data.ShopInfo.name;
            _data = data;
            MapCellManager = manager;
            _guid = Guid.NewGuid();
            OnModified.AddListener(ReloadNextSource);
        }

        public async Task LoadDefault()
        {
            if(defaultLayout == null) return;
            
            foreach (var VARIABLE in defaultLayout.SourceDto.Simulation.MachineStates)
            {
                Debug.Log(VARIABLE.Id);
            }
            var sim = defaultLayout.SourceDto.Simulation;

            try
            {
                Debug.Log(defaultLayout.SourceDto.Guid);
                sim.RehydrateSourceReferences(new Dictionary<Guid, Source>()
                    { { defaultLayout.SourceDto.Guid, this } });
                var realSim = new FactoryGridSimulation(this);
                await realSim.LoadState(sim);
                SetSimulation(realSim);
                _lastLayout = defaultLayout.SourceDto.lastLayout;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }


            //throw new NotImplementedException("fuckign kill me");

        }

        private void Update()
        {
            if (!_placed)
            {
                MoveSource();
                CheckIfPlaced();
            }
        }

        
        #if UNITY_EDITOR
        [ContextMenu("Build Layout")]
        void BuildLayout()
        {
            if (_lastLayout == null) {
                if (_grid == null) {
                    Debug.Log("Couldn't save Layout cuz nothing to save.");
                    return;
                }

                _lastLayout = _grid.SaveCurrentLayout();
            }
            
            LayoutObject asset = ScriptableObject.CreateInstance<LayoutObject>();
            var dto = CaptureState();
            asset.Init(dto);

            string assetPath = "Assets/Prefab/DefaultLayouts/" + _data.name + "LayoutObject"+ ".asset";
            AssetDatabase.CreateAsset(asset, assetPath); // Save as an asset file
            AssetDatabase.SaveAssets(); // Ensure it's saved to disk
            AssetDatabase.Refresh(); // Refresh the Project window

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        #endif
        
        public Task<bool> PlacementProcess()
        {
            _placedProcess = new TaskCompletionSource<bool>();
            _placed = false;
            return _placedProcess.Task;
        }
        
        protected virtual void CheckIfPlaced()
        {
            if (Input.GetMouseButtonDown(0)) {
                Place(transform.position);
            }

            if (Input.GetMouseButtonDown(1)) {
                CancelPlacement();
            }
            
        }

        private void CancelPlacement()
        {
            if(_placedProcess == null) return;
            _placedProcess.TrySetResult(false);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected virtual void MoveSource()
        {
            Vector3 currentPos = MapCellManager.GetMouseCurrentPosition();
            if(currentPos.Equals(Vector3.negativeInfinity)) return;
            
            transform.position = new  Vector3(currentPos.x, currentPos.y, transform.position.z);
        }

        public virtual void Place(Vector2 pos)
        {
            _placed = true;
            _placedProcess?.TrySetResult(true);
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            LoadDefault();
        }

        public SourceUI CreateUI()
        {
            if (!_placed) {
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
            MapCellManager?.SetMapMode(MapMode.Normal);
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
        
        private bool LinkToSource(Source dest)
        {
            if (!dest.AddIncomingSource(this)) return false;

            if (_nextSource != null) _nextSource.RemoveIncomingSource(this);

            _nextSource = dest;
            return true;
        }

        private bool RemoveIncomingSource(Source source)
        {
            for (var i = 0; i < _incomingSources.Length; i++)
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

            for (var i = 0; i < _incomingSources.Length; i++)
            {
                if (_incomingSources[i] != null) continue;
                _incomingSources[i] = source;
                OnIncomingSourcesChanged?.Invoke();
                return true;
            }

            return false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnMouseDown();
        }
        
        private void OnMouseDown()
        {
            MapCellManager.SwitchUIPreview(this);
        }

        public Guid GetGuid()
        {
            return _guid;
        }
   


    }
}