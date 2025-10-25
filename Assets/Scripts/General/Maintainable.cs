using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class Maintainable : MonoBehaviour
    {
        [SerializeField] protected int upfrontCost;
        [SerializeField] protected int dailyCost;

        public int DailyCost => dailyCost;
        public int UpfrontCost => upfrontCost;
    }
}
