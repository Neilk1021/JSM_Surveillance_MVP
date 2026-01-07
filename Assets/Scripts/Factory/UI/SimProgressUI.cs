using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JSM.Surveillance.UI
{
    public class SimProgressUI : MonoBehaviour
    {
        [SerializeField] private Scrollbar scrollbar;
        
        private void OnEnable()
        {
            SurveillanceGameManager.instance?.Simulator?.OnTickProgress?.AddListener(UpdateSlider);
        }

        private void OnDisable()
        {
            SurveillanceGameManager.instance?.Simulator?.OnTickProgress?.RemoveListener(UpdateSlider);
        }

        private void UpdateSlider(float t)
        {
            scrollbar.value = Mathf.Clamp01(t);
        }
    }   
}

