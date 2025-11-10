using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    [Range(0.1f, 4)] [SerializeField] private float speed;
    [Range(0.25f, 4)] [SerializeField] private float scrollSpeed = 2;

    [SerializeField] private float maxZoom, minZoom;
    
    private float _startZ;
    private float _currentZoom = 1;
    
    private void Start()
    {
        _startZ = transform.localPosition.z;
    }

    private void Update()
    {
        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal")* speed * _currentZoom, Input.GetAxisRaw("Vertical")*speed * _currentZoom, 0);

        float z = 0;
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            z = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        }

        z = Mathf.Clamp(z + transform.localPosition.z,  _startZ + minZoom, _startZ + maxZoom);
        _currentZoom = Mathf.InverseLerp(  maxZoom+ _startZ, minZoom + _startZ,z)+0.5f;
        
        transform.localPosition += dir;
        transform.localPosition = new Vector3(transform.position.x, transform.localPosition.y, z);
    }
}
