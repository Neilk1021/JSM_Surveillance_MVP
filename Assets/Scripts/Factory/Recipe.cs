using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Surveillance.TechTree;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "JSM/Surveillance/Recipe")]

    public class Recipe : Unlockable 
    {
        [SerializeField] private string guid;
        [SerializeField] private string recipeName;
        [SerializeField] private List<ResourceVolume> inputVolumes;
        [SerializeField] private ResourceVolume outputVolume;
        [FormerlySerializedAs("time")] [SerializeField] private int ticks;

        public string Guid => guid;
        public string RecipeName => recipeName;
        public List<ResourceVolume> InputVolumes => inputVolumes;
        public ResourceVolume OutputVolume => outputVolume;
        public int Ticks => ticks;

        public bool RequiresInput(Resource resource)
        {
            return inputVolumes.Exists(x=> x.resource == resource);
        }
        
        public int RequiredAmount(Resource resource)
        {
            return inputVolumes.FirstOrDefault(x => x.resource == resource).amount;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            #if UNITY_EDITOR
                string path = AssetDatabase.GetAssetPath(this);
                guid = AssetDatabase.AssetPathToGUID(path);
            #endif
        }
    }
}