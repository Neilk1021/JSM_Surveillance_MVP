using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRenderDash : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Material lineMaterialInstance;

    private static readonly int Accumulation = Shader.PropertyToID("_Accumulation");
    private static readonly int ScrollSpeed = Shader.PropertyToID("_ScrollSpeed");

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            enabled = false;
            return;
        }

        lineMaterialInstance = lineRenderer.material;

        if (lineMaterialInstance == null)
        {
            enabled = false;
            return;
        }

        if (!lineMaterialInstance.HasProperty(Accumulation)) { }
        if (!lineMaterialInstance.HasProperty(ScrollSpeed)) { }

        float initialSpeed = lineMaterialInstance.GetFloat(ScrollSpeed);
        if (initialSpeed == 0f) { }
    }

    void Update()
    {
        if (lineMaterialInstance == null) return;

        float currentAccumulation = lineMaterialInstance.GetFloat(Accumulation);
        float currentScrollSpeed = lineMaterialInstance.GetFloat(ScrollSpeed);

        float newAccumulation = currentAccumulation + Time.deltaTime * currentScrollSpeed;

        lineMaterialInstance.SetFloat(Accumulation, newAccumulation);
    }

    void OnDestroy()
    {
        if (lineMaterialInstance != null)
        {
            if (lineRenderer != null && lineRenderer.sharedMaterial != lineMaterialInstance)
            {
                Destroy(lineMaterialInstance);
            }
        }
    }
}