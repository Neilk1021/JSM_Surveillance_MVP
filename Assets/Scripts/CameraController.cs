using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    [Range(5, 15)] [SerializeField] private float speed;
    [Range(100, 3000)] [SerializeField] private float scrollSpeed = 500;

    [SerializeField] private float maxZoom, minZoom;
    
    private float _startZ;
    private float _currentZoom = 1;
    private bool _scrollEnabled = true;
    
    private void Start()
    {
        _startZ = transform.localPosition.z;
    }

    private void Update()
    {
        if(!_scrollEnabled) return;
        
        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        float z = 0;
        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
        {
            z = Input.GetAxisRaw("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime;
        }

        z = Mathf.Clamp(z + transform.localPosition.z,  _startZ + minZoom, _startZ + maxZoom);
        _currentZoom = Mathf.InverseLerp(maxZoom + _startZ, minZoom + _startZ, z);
        
        transform.localPosition += dir * (Time.deltaTime * speed * Mathf.Lerp(0.9f, 1.125f,_currentZoom));
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
    }

    public void SetScrollActive(bool b)
    {
        _scrollEnabled = b;
    }
}
