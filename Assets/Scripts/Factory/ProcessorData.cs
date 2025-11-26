using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewProcessor", menuName = "JSM/Surveillance/Maintainable/Processor")]
    /// <summary>
    /// Base data and settings for the processor
    /// </summary>
    public class ProcessorData : Maintainable
    {
        [SerializeField] private RecipeBank recipeBank;
        [SerializeField] private float speed;

        public RecipeBank RecipeBank => recipeBank;
        public float Speed => speed;
    }
}