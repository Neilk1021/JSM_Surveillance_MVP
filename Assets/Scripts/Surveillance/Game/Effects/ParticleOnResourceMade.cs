using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance.Game.Effects
{
    public class ParticleOnResourceMade : MonoBehaviour
    {
        [SerializeField] private IconParticle iconParticlePrefab;
        [SerializeField] private float time; 
        [SerializeField] private float speed;
        [SerializeField] private Vector2 dir;
        [SerializeField] private Source source;

        private RectTransform _rectTransform;
        private void Start()
        {
            source.OnResourceMade += SpawnInParticle;
        }
        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        void LateUpdate()
        {
            _rectTransform.rotation = Quaternion.Euler(-10.7f, 0, 0);
        }
        

        public void SpawnInParticle(Resource resource)
        {
            var particle = Instantiate(iconParticlePrefab, transform);
            particle.Init(resource.Sprite, time, speed*dir.normalized);
        }
    }
}
