using Simulation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class HeatMapController : MonoBehaviour
    {
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

            var cam = UIState.Camera.Get();
            float4x4 camTransform = cam.projectionMatrix * cam.worldToCameraMatrix;
            
            foreach (var agent in SimulationController.Instance.AgentManager.GetAgents())
            {
                var clipSpacePos = math.mul(
                    camTransform,
                    new float4(agent.transform.position, 1f)
                );
                clipSpacePos /= clipSpacePos.w;
                
                var screenSpacePos = clipSpacePos.xyz / 2f + 0.5f;
                
                if (screenSpacePos.x >= 0f && screenSpacePos.x <= 1f &&
                    screenSpacePos.y >= 0f && screenSpacePos.y <= 1f &&
                    screenSpacePos.z <= 1f)
                {
                    // Normalize to fit into bins
                    screenSpacePos *= HeatMapResolution - 1;
                    
                    var frac = math.frac(screenSpacePos.xy);
                    var floor = new int2(screenSpacePos.xy);
                    
                    agentPositionGrid[floor.x +       floor.y * HeatMapResolution]       += (1f - frac.x) * (1f - frac.y);
                    agentPositionGrid[floor.x +       (floor.y + 1) * HeatMapResolution] += (1f - frac.x) * frac.y;
                    agentPositionGrid[(floor.x + 1) + floor.y * HeatMapResolution]       += frac.x        * (1f - frac.y);
                    agentPositionGrid[(floor.x + 1) + (floor.y + 1) * HeatMapResolution] += frac.x        * frac.y;
                }
            }

            //    Update shader uniform
            material.SetFloatArray("_AgentPositionGrid", agentPositionGrid);
        }
    }
}