using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    
    [System.Serializable]
    public class ShopInfo 
    {
        public string name;
        public string desc; 
        public GameObject itemModelPrefab;
    }
    
    public abstract class Maintainable : ScriptableObject 
    {
        [SerializeField] protected int upfrontCost;
        [SerializeField] protected int dailyCost;

        public int DailyCost => dailyCost;
        public int UpfrontCost => upfrontCost;
        
        
        [SerializeField] private ShopInfo shopInfo;

        public ShopInfo ShopInfo => shopInfo;
    }
}
