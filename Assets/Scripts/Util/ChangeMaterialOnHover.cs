using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


namespace JSM.Surveillance.Util
{
    public class ChangeMaterialOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Mats")]
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material highlightedMaterial;

        [Header("Rendering")]
        [SerializeField] MeshRenderer meshRenderer;

        private void SetMaterial(Material mat)
        {
            meshRenderer ??= GetComponent<MeshRenderer>();
            meshRenderer.material = mat;
        }
        
        private void Start()
        {
            SetMaterial(normalMaterial);
        }

        private void OnMouseEnter()
        {
            SetMaterial(highlightedMaterial);
        }

        private void OnMouseExit()
        {
            SetMaterial(normalMaterial);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
           OnMouseEnter(); 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnMouseExit();
        }
    }
}