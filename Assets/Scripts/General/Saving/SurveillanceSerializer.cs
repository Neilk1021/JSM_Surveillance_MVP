using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JSM.Surveillance;
using JSM.Surveillance.Game;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JSM.Surviellance.Saving
{
    public class SurveillanceSerializer : MonoBehaviour
    {
        [SerializeField] private MapCellManager mapCellManager;

        [Header("Save Settings")]
        [SerializeField] private bool useTestSavePath = false;
        [SerializeField] private string testFileName = "test_factory.dat";
        [SerializeField] private string prodFileName = "factory_save.dat";

        /// <summary>
        /// Dynamically determines which path to use based on the toggle.
        /// </summary>
        private string SavePath 
        {
            get 
            {
                string fileName = useTestSavePath ? testFileName : prodFileName;
            
            #if UNITY_EDITOR
                return Path.Combine(Application.dataPath, "..", "Saves", fileName);
            #else
            return Path.Combine(Application.persistentDataPath, fileName);
            #endif
            }
        }

        private void EnsureDirectoryExists()
        {
            string dir = Path.GetDirectoryName(SavePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }


        [ContextMenu("Save Game")]
        public async void SaveGame()
        {
            string directory = Path.GetDirectoryName(SavePath);
            if (!Directory.Exists(directory)) 
            {
                Directory.CreateDirectory(directory);
            }
            
            var sources = FindObjectsOfType<Source>();
            
            using (var stream = File.Open(SavePath, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
            {
                // Write Header
                writer.Write(1); // Version
                writer.Write(sources.Length);
                writer.Write(SurveillanceGameManager.GetMoney());

                foreach (var source in sources)
                {
                    var dto = (SourceDTO)source.CaptureState();
                    dto.Write(writer);
                }
            }
            Debug.Log($"Saved {sources.Length} sources to {SavePath}");
        }
        
        [ContextMenu("Load Game")]
        public async void LoadGame()
        {
            if (!File.Exists(SavePath)) return;

            using (var stream = File.Open(SavePath, FileMode.Open))
            using (var reader = new BinaryReader(stream))
            {
                int version = reader.ReadInt32();
                int sourceCount = reader.ReadInt32();
                int money = reader.ReadInt32();

                SurveillanceGameManager.SetMoney(money);

                List<SourceDTO> dtos = new List<SourceDTO>();
                for (int i = 0; i < sourceCount; i++)
                {
                    SourceDTOType type = (SourceDTOType)reader.ReadByte();

                    SourceDTO dto;
                    switch (type)
                    {
                        case SourceDTOType.Normal:
                            dto = new SourceDTO();
                            break;
                        case SourceDTOType.Mic:
                            dto = new SourceDTO.MicDTO();
                            break;
                        case SourceDTOType.Camera:
                            dto = new SourceDTO.CameraDTO();
                            break;
                        default:
                            throw new ArgumentException("Invalid Type");
                            
                    }
                    
                    dto.Read(reader);
                    dtos.Add(dto);
                }


                SurveillanceGameManager.ClearBoard();
                await ExecuteLoadPasses(dtos);
            }
        }


        private async Task ExecuteLoadPasses(List<SourceDTO> dtos)
        {
            Dictionary<Guid, Source> sourceLookup = await SpawnPass(dtos);
            await RehydratePass(dtos, sourceLookup);
            Debug.Log("Load Complete!");
        }

        private async Task<Dictionary<Guid, Source>> SpawnPass(List<SourceDTO> dtos)
        {
            var lookup = new Dictionary<Guid, Source>();

            var sourceDataList = new List<(AsyncOperationHandle<SourceData> task, SourceDTO dto)>();
            
            foreach (var dto in dtos)
            {
                sourceDataList.Add((Addressables.LoadAssetAsync<SourceData>(dto.sourceDataPath), dto));
            }

            await Task.WhenAll(sourceDataList.Select(x => x.task.Task).ToList());

            foreach (var tuple in sourceDataList.Where(x=> x.task.Status == AsyncOperationStatus.Succeeded))
            {
                Source newSource = SurveillanceGameManager.SpawnSourceImmediate(tuple.task.Result);
                await newSource.LoadState(tuple.dto);
                lookup.Add(newSource.GetGuid(), newSource);
            }
            
            return lookup;
        }

        private async Task RehydratePass(List<SourceDTO> dtos, Dictionary<Guid, Source> lookup)
        {
            foreach (var dto in dtos)
            {
                if (lookup.TryGetValue(dto.Guid, out Source source))
                {
                    await source.RehydrateState(dto, lookup);
                }
            }
        } 
    }   
}


