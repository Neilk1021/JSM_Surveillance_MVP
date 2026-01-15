using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    
    [System.Serializable]
    public class ShopInfo 
    {
        public string name;
        [TextArea(minLines:5, maxLines:10)]
        public string desc; 
        public GameObject itemModelPrefab;
    }
    
    [CreateAssetMenu(fileName = "Generic", menuName = "JSM/Surveillance/Maintainable/Generic")]

    public class Maintainable : ScriptableObject 
    {
        [SerializeField] protected int upfrontCost;
        [SerializeField] protected int dailyCost;

        public int DailyCost => dailyCost;
        public int UpfrontCost => upfrontCost;
        
        
        [SerializeField] private ShopInfo shopInfo;

        public ShopInfo ShopInfo => shopInfo;
    }
}
