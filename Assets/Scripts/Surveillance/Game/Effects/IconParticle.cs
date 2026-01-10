using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconParticle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image img;
    [SerializeField] private Rigidbody rb; 
    private Vector2 _vel;

    private static string TurnIntToMoney(int money)
    {
        return $"¥{money:N0}";
    }
    
    public void Init(int money, float time, Vector2 vel)
    {
        text.text = TurnIntToMoney(money); 
        Init(time,vel);
    }
    
    public void Init(Sprite icon, float time, Vector2 vel)
    {
        Init(time, vel);
        img.sprite = icon;
    }

    private void Init(float time, Vector2 vel)
    {
        _vel = vel;
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
