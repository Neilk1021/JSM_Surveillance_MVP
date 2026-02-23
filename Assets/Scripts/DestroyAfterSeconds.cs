using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float seconds = 1;

    private void Start()
    {
        Destroy(gameObject, seconds);
    }
}
