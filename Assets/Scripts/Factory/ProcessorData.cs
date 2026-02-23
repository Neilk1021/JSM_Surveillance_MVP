using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewProcessor", menuName = "JSM/Surveillance/Maintainable/Processor")]
    public class ProcessorData : MachineSObj, ISerializationCallbackReceiver
    {

        [SerializeField] private RecipeBank recipeBank;
        [SerializeField] private float speed;

        public RecipeBank RecipeBank => recipeBank;
        public float Speed => speed;
    }
}