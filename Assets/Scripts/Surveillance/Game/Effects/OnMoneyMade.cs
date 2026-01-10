using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.Game.Effects
{
    public class OnMoneyMade : MonoBehaviour
    {
        [SerializeField] private IconParticle iconParticlePrefab;
        [SerializeField] private float time;
        [SerializeField] private float speed;
        [SerializeField] private Vector2 dir;
        [SerializeField] private Source source;

        private RectTransform _rectTransform;

        private void Start()
        {
            source.OnMoneyEarned += SpawnInMoneyParticle;
        }

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        void LateUpdate()
        {
            _rectTransform.rotation = Quaternion.Euler(-10.7f, 0, 0);
        }

        public void SpawnInMoneyParticle(int money)
        {
            var particle = Instantiate(iconParticlePrefab, transform);
            particle.Init(money, time, speed * dir.normalized);
        }
    }
}