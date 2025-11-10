using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePreviewObject : MonoBehaviour
{
    [Range(1, 20)] [SerializeField] private float speed;
    void Update()
    {
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            speed*Time.deltaTime + transform.rotation.eulerAngles.y, 
            transform.rotation.eulerAngles.z
            );
    }
}
