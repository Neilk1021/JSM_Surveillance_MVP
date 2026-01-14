using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JSM.Surveillance.UI
{
    public class ItemFillAmntUI : MonoBehaviour
    {
        [Header("Fill Colors")] [SerializeField]
        private Color satisfiedColor;

        [SerializeField] private Color starvedColor;
        
        [Header("References")]
        [SerializeField] private Slider slider;
        [SerializeField] private Image fillUI;
        [SerializeField] private TextMeshProUGUI amntText;

        private int _prevCurrent;
        private int _prevMax;

        

        public void UpdateSlider(int current, int max, string itemName = "", bool starved = false)
        {
            if (current != _prevCurrent || _prevMax != max) {
                slider.value = (float)current / (float)max;
                _prevMax = max;
                _prevCurrent = current;
                amntText.text = itemName == "" ? $"{current}" : $"{itemName} - {current}";
            }
            
            fillUI.color = starved ? starvedColor : satisfiedColor;
        }
    }   
}

