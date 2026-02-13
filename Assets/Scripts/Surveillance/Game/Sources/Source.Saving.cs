using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JSM.Surveillance.Saving;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JSM.Surveillance.Game
{
    public abstract partial class Source
    {
        public virtual SourceDTO CaptureState()
        {
            SimulationSaveData sim = (SimulationSaveData)_simulation?.CaptureState();
            Guid nextSourceGuid = (_nextSource != null) ? _nextSource._guid : Guid.Empty;
            List<Guid> incomingGuids = new List<Guid>();
            string sourcePath = _data.AssetGuid;
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
                sourceDataPath = sourcePath, 
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
            Debug.Log("ahh");
            
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