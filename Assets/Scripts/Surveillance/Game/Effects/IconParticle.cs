using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconParticle : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private Rigidbody rb; 
    private Vector2 _vel;

    public void Init(Sprite icon, float time, Vector2 vel)
    {
        _vel = vel;
        img.sprite = icon;
        Destroy(gameObject, time);
        transform.position += (Vector3)(vel*(0.5f));
    }

    private void Awake()
    {
        rb ??= GetComponent<Rigidbody>();
        img ??= GetComponentInChildren<Image>();
    }

    private void Update()
    {
        rb.velocity = transform.TransformDirection(_vel);
    }
}
