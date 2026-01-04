using System;
using System.Linq;
using JSM.Surveillance.UI;
using UnityEngine;

namespace JSM.Surveillance.Game
{
    public enum MapMode
    {
        Normal,
        Population,
        Risk, 
        Consumer,
        Corp,
        Govt,
        Placement
    }
    
    public class MapRenderingManager : MonoBehaviour
    {
        [SerializeField] private int maxPop = 50;
        [SerializeField] private Gradient populationGradient;
        [SerializeField] private Gradient riskGradient;
        [SerializeField] private Gradient consumerGradient;
        [SerializeField] private Gradient govtGradient;
        [SerializeField] private Gradient corpGradient;
        [SerializeField] private Color buildingColor = new Color(0.01f, 0.04f, 0.07f);
        
        [SerializeField] [HideInInspector] private MapCellRendering[] cellRenderers;

        public static MapRenderingManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                return;
            }
            
            Destroy(this);
        }

        public void Init(MapCellRendering[] newCellRenderers)
        {
            cellRenderers = newCellRenderers;
        }

        private void Start()
        {
            if (cellRenderers != null)
            {
                maxPop = FindMaxPop();
            }
        }

        private int FindMaxPop()
        {
            return cellRenderers.Max(x =>
            {
                return x.Cell.GetData().DailyPopulation;
            });
        }

        public void ChangeRendering(MapMode mode) {
            //lazy cache if needed
            cellRenderers ??= FindObjectsOfType<MapCellRendering>();
            Color color;
            
            foreach (var mapCell in cellRenderers)
            {
                var data = mapCell.Cell.GetData();
                mapCell.SetMode(mode);
                switch (mode)
                {
                    case MapMode.Normal:
                        mapCell.ResetColor();
                        break;
                    case MapMode.Population:
                        color = mapCell.Cell.IsStreet ? populationGradient.Evaluate((float)data.DailyPopulation/maxPop) : new Color(0.02f, 0.45f, 0.05f);
                        mapCell.SetColor(!mapCell.Cell.IsStreet ? buildingColor : color, color);
                        mapCell.SetBorderColor(new Color(0.05f,0.5f,0.1f));
                        break;
                    case MapMode.Risk:
                        color = mapCell.Cell.IsStreet ? riskGradient.Evaluate(data.RiskFactor) : new Color(0.45f, 0.05f, 0.02f);
                        mapCell.SetColor(!mapCell.Cell.IsStreet ? buildingColor : color, color);
                        mapCell.SetBorderColor(new Color(0.5f,0.1f,0.05f));
                        break;
                    case MapMode.Consumer:
                        color = mapCell.Cell.IsStreet ? consumerGradient.Evaluate(data.Ratio.consumer) : new Color(0.4f, 0.45f, 0.00f);
                        mapCell.SetColor(!mapCell.Cell.IsStreet ? buildingColor : color, color);
                        mapCell.SetBorderColor(new Color(0.5f,0.5f,0.1f));
                        break;
                    case MapMode.Corp:
                        color = mapCell.Cell.IsStreet ? corpGradient.Evaluate(data.Ratio.corporate) : new Color(0.2f, 0.5f, 0.5f);
                        mapCell.SetColor(!mapCell.Cell.IsStreet ? buildingColor : color, color);
                        mapCell.SetBorderColor(new Color(0.3f,0.6f,0.6f));
                        break;
                    case MapMode.Govt:
                        color = mapCell.Cell.IsStreet ? govtGradient.Evaluate(data.Ratio.government) : new Color(0.5f, 0.05f, 0.5f);
                        mapCell.SetColor(!mapCell.Cell.IsStreet ? buildingColor : color, color);
                        mapCell.SetBorderColor(new Color(0.7f,0.1f,0.7f));
                        break;

                    
                }
            }
        }
    }
}