using System;
using Simulation;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeatMapController : MonoBehaviour
    {
        public CrowdManager CrowdManager;
        public Vector3 TopLeft, BottomRight;
        private const int HeatMapGranularity = 32;
        private Vector3 poolDimensions;
        private readonly float[] agentHeatMap = new float[HeatMapGranularity * HeatMapGranularity];

        private void Start()
        {
            GetComponent<Image>().material.SetInt("_HeatMapGranularity", HeatMapGranularity);
        }

        private void OnGUI()
        {
            poolDimensions = (TopLeft - BottomRight) / HeatMapGranularity;

            Array.Clear(agentHeatMap, 0, agentHeatMap.Length);
            
            CrowdManager.GetAgents().ForEach(agent =>
            {
                Vector3 pos = agent.transform.position;
                if (pos.x >= TopLeft.x && pos.x <= BottomRight.x && pos.z <= TopLeft.z && pos.z >= BottomRight.z)
                {
                    Vector3 relativePosition = (TopLeft - agent.transform.position);
                
                    int x = Mathf.FloorToInt(relativePosition.x / poolDimensions.x);
                    int z = Mathf.FloorToInt(relativePosition.z / poolDimensions.z);

                    agentHeatMap[x + z * HeatMapGranularity]++;
                }
            });

            GetComponent<Image>().material.SetFloatArray("_AgentHeatMap", agentHeatMap);
        }

        private void OnDrawGizmosSelected()
        {
            //  Visualize the area
            Gizmos.color = new Color(1, 0, 0, 0.7f);
            Gizmos.DrawCube(
                (TopLeft + BottomRight) / 2 + Vector3.up * 30,
                TopLeft - BottomRight
            );
        }
    }
}