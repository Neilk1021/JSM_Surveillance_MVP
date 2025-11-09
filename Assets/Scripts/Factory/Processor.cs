using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewProcessor", menuName = "Factory/Processor")]
    public class Processor : ScriptableObject
    {
        [SerializeField] private RecipeBank recipeBank;
        [SerializeField] private float speed;
        [SerializeField, HideInInspector] private Recipe selectedRecipe;
        private Dictionary<Resource, int> inputResources;
        private Dictionary<Resource, int> outputResources;

        public RecipeBank RecipeBank => recipeBank;
        public float Speed => speed;
        public Recipe SelectedRecipe => selectedRecipe;
        public IReadOnlyDictionary<Resource, int> InputResources => inputResources;
        public IReadOnlyDictionary<Resource, int> OutputResources => outputResources;


        public void SelectRecipe(Recipe recipe)
        {
            selectedRecipe = recipe;
            CalculateResourceSummary();
        }

        public void CalculateResourceSummary()
        {
            inputResources = new Dictionary<Resource, int>();
            outputResources = new Dictionary<Resource, int>();

            if (selectedRecipe == null) return;

            foreach (var input in selectedRecipe.inputVolume)
            {
                if (!inputResources.ContainsKey(input.resource))
                    inputResources[input.resource] = 0;
                inputResources[input.resource] += input.amount;
            }

            foreach (var output in selectedRecipe.outputVolume)
            {
                if (!outputResources.ContainsKey(output.resource))
                    outputResources[output.resource] = 0;
                outputResources[output.resource] += output.amount;
            }
        }
    }
}