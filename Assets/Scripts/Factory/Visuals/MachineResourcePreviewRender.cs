using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MachineObject))]
public class MachineResourcePreviewRender : MonoBehaviour
{
    [SerializeField] private Image resourceImage;
    private MachineObject _machineObject;

    private void Awake() {
        _machineObject = GetComponent<MachineObject>();
    }

    private void OnEnable()
    {
        _machineObject.OnModify += Load;
        Load();
    }

    private void OnDisable()
    {
        _machineObject.OnModify -= Load;
    }

    public void Load()
    {
        var resource = _machineObject.GetResource();
        
        if (resource == null) {
            resourceImage.sprite = null;
            resourceImage.color = Color.clear;
            return; 
        }
        
        resourceImage.color = Color.white;
        resourceImage.sprite = resource.Sprite;
    }
}
