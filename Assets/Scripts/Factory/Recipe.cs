using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "JSM/Surveillance/Recipe")]

    public class Recipe : ScriptableObject
    {
        [SerializeField] private string guid = System.Guid.NewGuid().ToString();
        [SerializeField] private string recipeName;
        [SerializeField] private ResourceVolume[] inputVolume;
        [SerializeField] private ResourceVolume[] outputVolume;
        [SerializeField] private float time;

        public string Guid => guid;
        public string RecipeName => recipeName;
        public ResourceVolume[] InputVolume => inputVolume;
        public ResourceVolume[] OutputVolume => outputVolume;
        public float Time => time;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(guid))
                guid = System.Guid.NewGuid().ToString();
        }
    }
}