using Simulation;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeatMapController : MonoBehaviour
    {
        public AgentManager AgentManager;
        public Vector3 BottomLeftCorner;
        public float CaptureArea;
        private const int HeatMapResolution = 31;
        private readonly float[] agentPositionGrid = new float[HeatMapResolution * HeatMapResolution];
        public float AgentPersistenceDecrease;
        
        private Material material;

        private void Awake()
        {
            material = GetComponent<Image>().material;
        }

        private void OnGUI()
        {
            //    Slowly decrease the agent counts
            for (var i = 0; i < agentPositionGrid.Length; i++)
            {
                agentPositionGrid[i] *= AgentPersistenceDecrease;
            }
            
            //    Put each agent's position into the position grid
            var captureArea = new Vector3(CaptureArea, 0, CaptureArea);
            var poolDimensions = captureArea / (HeatMapResolution - 1);
            var topRightCorner = BottomLeftCorner + captureArea;
            foreach (var agent in AgentManager.GetAgents())
            {
                var pos = agent.transform.position;
                if (pos.x >= BottomLeftCorner.x &&
                    pos.x <= topRightCorner.x &&
                    pos.z >= BottomLeftCorner.z &&
                    pos.z <= topRightCorner.z)
                {
                    var relativePosition = topRightCorner - agent.transform.position;
                    //    These are between 0 and (HeatMapResolution - 1)
                    var x = relativePosition.x / poolDimensions.x;
                    var z = relativePosition.z / poolDimensions.z;
                    
                    var xfrac = x - Mathf.Floor(x);
                    var zfrac = z - Mathf.Floor(z);

                    var xfloor = Mathf.FloorToInt(x);
                    var zfloor = Mathf.FloorToInt(z);
                    
                    agentPositionGrid[xfloor +       zfloor * HeatMapResolution]       += (1f - xfrac) * (1f - zfrac);
                    agentPositionGrid[xfloor +       (zfloor + 1) * HeatMapResolution] += (1f - xfrac) * zfrac;
                    agentPositionGrid[(xfloor + 1) + zfloor * HeatMapResolution]       += xfrac        * (1f - zfrac);
                    agentPositionGrid[(xfloor + 1) + (zfloor + 1) * HeatMapResolution] += xfrac        * zfrac;
                }
            }

            //    Update shader uniform
            material.SetFloatArray("_AgentPositionGrid", agentPositionGrid);
        }

        private void OnDrawGizmosSelected()
        {
            //  Visualize the area
            Gizmos.color = new Color(1, 0, 0, 0.7f);
            var captureArea = new Vector3(CaptureArea, 0, CaptureArea);
            var topRightCorner = BottomLeftCorner + captureArea;
            Gizmos.DrawCube(
                (BottomLeftCorner + topRightCorner) / 2 + Vector3.up * 30,
                captureArea
            );
        }
    }
}