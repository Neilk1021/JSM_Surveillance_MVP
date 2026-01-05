using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EfficientResScaler : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;
    [SerializeField] private RawImage displayUI;
    [SerializeField] private int targetHeight = 360;
    [SerializeField] public Camera rtCam;

    public RawImage DisplayUI => displayUI;

    public static EfficientResScaler instance;

    private void Awake()
    {
        if (instance) {
            Destroy(gameObject);
        }

        instance = this;
    }


    private void Start() => UpdateSize();

    private void Update()
    {
        // Only check Screen.height to detect resolution or aspect ratio changes
        if (worldCamera.targetTexture == null || Screen.height != worldCamera.targetTexture.height * (Screen.height / targetHeight))
        {
            UpdateSize();
        }
    }

    void UpdateSize()
    {
        if (worldCamera.targetTexture != null)
        {
            worldCamera.targetTexture.Release();
        }

        float aspect = (float)Screen.width / Screen.height;
        int targetWidth = Mathf.RoundToInt(targetHeight * aspect);

        // Create a temporary descriptor for better performance in URP/HDRP
        RenderTexture rt = new RenderTexture(targetWidth, targetHeight, 24, RenderTextureFormat.Default);
        rt.filterMode = FilterMode.Point;
        rt.Create();

        worldCamera.targetTexture = rt;
        displayUI.texture = rt;
    }
    
    private void OnDestroy()
    {
        if(worldCamera == null) return;
        
        if (worldCamera.targetTexture != null)
        {
            worldCamera.targetTexture.Release();
            //Destroy(worldCamera.targetTexture);
        }
    }
}