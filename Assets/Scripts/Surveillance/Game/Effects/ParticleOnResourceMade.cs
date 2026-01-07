using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance.Game.Effects
{
    public class ParticleOnResourceMade : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private Vector2 dir;
        [SerializeField] private Source source;
        
        private void Start()
        {
            source.OnResourceMade += SpawnInParticle;
        }

        public void SpawnInParticle(Resource resource)
        {
            //TODO implement the actual particle?
        }
    }
}
