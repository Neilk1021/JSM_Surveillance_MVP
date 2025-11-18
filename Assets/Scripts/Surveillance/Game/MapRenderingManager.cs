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
        Risk
    }
    
    public class MapRenderingManager : MonoBehaviour
    {
        [SerializeField] private int maxPop = 50;
        [SerializeField] private Gradient populationGradient;
        [SerializeField] private Gradient riskGradient;
        [SerializeField] private Color buildingColor = new Color(0.01f, 0.04f, 0.07f);
        
        [SerializeField] [HideInInspector] private MapCellRendering[] cellRenderers;
        public void Init(MapCellRendering[] newCellRenderers)
        {
            cellRenderers = newCellRenderers;
            maxPop = FindMaxPop();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeRendering(MapMode.Normal);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeRendering(MapMode.Population);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeRendering(MapMode.Risk);
            }
        }

        private int FindMaxPop()
        {
            return cellRenderers.Max(x => x.Cell.GetData().DailyPopulation);
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
                }
            }
        }
    }
}