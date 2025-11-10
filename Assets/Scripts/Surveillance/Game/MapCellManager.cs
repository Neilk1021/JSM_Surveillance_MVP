using System;
using System.Collections.Generic;
using JSM.Surveillance.Surveillance;
using UnityEngine;
using UnityEngine.Serialization;

namespace Surveillance.Game
{
    public class MapCellManager : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] GameObject companyTextPrefab;
        
        public GameObject CompanyTextPrefab => companyTextPrefab;

        [SerializeField] [HideInInspector] private List<HEGraphData.Vertex> vertices;
        [SerializeField] [HideInInspector] private List<HEHalf> halfEdges;
        
        /// <summary>
        /// Gets a given vertex.;
        /// </summary>
        /// <param name="pos">Vertex to selct</param>
        /// <returns>Vertex at position pos</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public HEGraphData.Vertex GetVertex(int pos)
        {
            if (pos >= vertices.Count || pos < 0)
            {
                throw new ArgumentOutOfRangeException($"{pos} out of range of verticies");
            }
            
            return vertices[pos];
        }
        
        /// <summary>
        /// Initializes the MapCellManager;
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="halves"></param>
        public void Init(List<HEGraphData.Vertex> verts, List<HEHalf> halves)
        {
            vertices = verts;
            halfEdges = halves;
        }
    }
}