using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace JSM.Surveillance.UI
{
    [RequireComponent(typeof(SortingGroup))]
    public class SurveillanceWindow : MonoBehaviour, IPointerDownHandler
    {
        private static int _globalSortOrder = 0;
        private Dictionary<Canvas, int> _canvases;
        private SortingGroup _group;
        private int _startingGroupIndex;
        private const int ReserveSpace = 50;
        public static int GlobalSortOrder => _globalSortOrder;

        private void Awake()
        {
            _group = GetComponent<SortingGroup>();
            _startingGroupIndex = _group.sortingOrder;
            _canvases = GetComponentsInChildren<Canvas>().ToDictionary(x => x, x => x.sortingOrder);
        }

        public void UpdateSortingLayer(int newLayer)
        {
            foreach (var canvas in transform.GetComponentsInChildren<Canvas>())
            {
                if(_canvases.ContainsKey(canvas)) continue;
                _canvases[canvas] = canvas.sortingOrder;
            }
            
            foreach (var kvp in _canvases) {
                kvp.Key.sortingOrder = kvp.Value + newLayer;
            }

            _group.sortingOrder = newLayer + _startingGroupIndex;
        }

        public void BringToFront()
        {
            _globalSortOrder += ReserveSpace;
            UpdateSortingLayer(_globalSortOrder);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            BringToFront();
        }
    }
}