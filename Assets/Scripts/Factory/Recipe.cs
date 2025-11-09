using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Factory/Recipe")]

    public class Recipe : ScriptableObject
    {
        public ResourceVolume[] inputVolume;
        public ResourceVolume[] outputVolume;
        public float time;
    }
}