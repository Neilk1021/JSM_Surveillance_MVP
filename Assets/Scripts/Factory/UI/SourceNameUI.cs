using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JSM.Surveillance.UI
{
    public class SourceNameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI sourceName;
        [SerializeField] private TMP_InputField inputField;

        private Source _source;
        private bool _changingName = false;


        private void Start()
        {
            _source = GetComponentInParent<FactoryGrid>().Source;
            UpdateActiveObjects();
            inputField.onSubmit.AddListener(x=>SwitchMode());
        }

        public void SwitchMode()
        {
            if (_changingName)
            {
                _source.UpdateName(inputField.text);
                sourceName.text = _source.SourceName;
                LayoutRebuilder.ForceRebuildLayoutImmediate(sourceName.rectTransform);

                _changingName = false;
                UpdateActiveObjects();
                return;
            }
            
            
            _changingName = true;
            UpdateActiveObjects();
            inputField.text = _source.SourceName;

        }

        private void UpdateActiveObjects()
        {
            if (Camera.main != null) Camera.main.GetComponent<CameraController>().enabled = !_changingName;

            inputField.GetComponent<CanvasGroup>().alpha = _changingName ? 1 : 0;
            inputField.GetComponent<CanvasGroup>().interactable = _changingName;
            inputField.GetComponent<CanvasGroup>().blocksRaycasts = _changingName;
            sourceName.GetComponent<CanvasGroup>().alpha = _changingName ? 0 : 1;
            sourceName.GetComponent<CanvasGroup>().interactable = !_changingName;
            sourceName.GetComponent<CanvasGroup>().blocksRaycasts = !_changingName;

        }
    }
}
