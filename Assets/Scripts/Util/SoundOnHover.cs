using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace JSM.Surveillance.Util
{
    public class SoundOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [FormerlySerializedAs("soundId")] [SerializeField] private string soundOnHoverId = "hover";
        [SerializeField] private string soundOnClickId = "click";
        private void OnMouseEnter()
        {
            SoundManager.PlaySound(soundOnHoverId);
        }

        private void OnMouseExit()
        {
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
           OnMouseEnter(); 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnMouseExit();
        }

        private void OnMouseDown()
        {
            SoundManager.PlaySound(soundOnClickId); 
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SoundManager.PlaySound(soundOnClickId);
        }
    }
}