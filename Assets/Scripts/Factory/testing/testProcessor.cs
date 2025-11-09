using UnityEngine;
using JSM.Surveillance;

public class ProcessorSceneTest : MonoBehaviour
{
    public Processor processor;

    void Start()
    {
        if (processor == null)
        {
            Debug.LogError("Processor not assigned.");
            return;
        }

        var recipe = processor.RecipeBank.GetRecipeByName("Rab"); //adjust name
        if (recipe == null)
        {
            Debug.LogError("Recipe not found.");
            return;
        }

        processor.SelectRecipe(recipe);

        Debug.Log("Inputs:");
        foreach (var entry in processor.InputResources)
            Debug.Log($"{entry.Key.resourceName}: {entry.Value}");

        Debug.Log("Outputs:");
        foreach (var entry in processor.OutputResources)
            Debug.Log($"{entry.Key.resourceName}: {entry.Value}");
    }
}
