using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSM.Surveillance.UI
{
    public class FactoryGridBackgroundFader : MonoBehaviour
    {
        private static readonly int Hide = Animator.StringToHash("Hide");
        private static readonly int Show = Animator.StringToHash("Show");
        [SerializeField] private Canvas canvas;
        [SerializeField] private Animator animator;
        [SerializeField] private float secondsToDestroy = 1f;
        
        
        private void Awake() {
            canvas.worldCamera = GetComponentInParent<Camera>();
        }

        public void Close()
        {
            animator.SetTrigger(Hide);
            Destroy(gameObject, secondsToDestroy);
        }

        public void ShowFade()
        {
            animator.SetTrigger(Show);
        }

        public void HideFade()
        {
            animator.SetTrigger(Hide);
        }
        
    }
}

