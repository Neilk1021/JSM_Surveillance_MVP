using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class CompanyNameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private RectTransform rect;
        
        public void Resize(Bounds bounds)
        {
            float width = bounds.max.x - bounds.min.x;
            float height = bounds.max.y - bounds.min.y;

            width = Mathf.Min(width, rect.rect.width);
            height = Mathf.Min(height, rect.rect.height);
            rect.sizeDelta = new Vector2(width, height);
        }
        
        public void SetCompanyName(string newName)
        {
            this.text.text = newName;
        }
        
        
    }   
}

