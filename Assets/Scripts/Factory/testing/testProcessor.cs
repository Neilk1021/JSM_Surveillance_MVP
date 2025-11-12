using UnityEngine;
using JSM.Surveillance;
using System.Collections.Generic;

public class ProcessorSceneTest : MonoBehaviour
{
    [SerializeField] private ProcessorData processorData;
    [SerializeField] private Recipe recipeToTest;
    [SerializeField] private Resource resourceA;
    [SerializeField] private Resource resourceB;

    private float progress;
    private float timer;

    // local runtime state (since ProcessorInstance is abstract)
    private Dictionary<Resource, int> input = new();
    private Dictionary<Resource, int> output = new();
    private bool isRunning;

    void Start()
    {
        if (!processorData || !recipeToTest)
        {
            Debug.LogError("Assign ProcessorData and Recipe in the inspector!");
            enabled = false;
            return;
        }

        // initial feed
        input[resourceA] = 10;
        input[resourceB] = 6;

        Debug.Log($"Processor started with recipe: {recipeToTest.name}");
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        if (!isRunning && InputSatisfied())
        {
            ConsumeInputs();
            isRunning = true;
            progress = 0f;
        }

        if (isRunning)
        {
            progress += deltaTime * processorData.Speed;
            if (progress >= recipeToTest.Time)
            {
                ProduceOutputs();
                isRunning = false;
                progress = 0f;
            }
        }

        // print once per second
        timer += deltaTime;
        if (timer >= 1f)
        {
            timer = 0f;
            PrintState();
        }
    }

    private bool InputSatisfied()
    {
        foreach (var r in recipeToTest.InputVolume)
            if (!input.ContainsKey(r.resource) || input[r.resource] < r.amount)
                return false;
        return true;
    }

    private void ConsumeInputs()
    {
        foreach (var r in recipeToTest.InputVolume)
            input[r.resource] -= r.amount;
    }

    private void ProduceOutputs()
    {
        foreach (var r in recipeToTest.OutputVolume)
        {
            if (!output.ContainsKey(r.resource))
                output[r.resource] = 0;
            output[r.resource] += r.amount;
        }
    }

    private void PrintState()
    {
        foreach (var kv in input)
            Debug.Log($"input {kv.Key.ResourceName}: {kv.Value}");
        foreach (var kv in output)
            Debug.Log($"output {kv.Key.ResourceName}: {kv.Value}");
    }
}
