using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "JSM/Surveillance/Recipe")]

    public class Recipe : ScriptableObject
    {
        [SerializeField] private string guid = System.Guid.NewGuid().ToString();
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
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(guid))
                guid = System.Guid.NewGuid().ToString();
        }
    }
}