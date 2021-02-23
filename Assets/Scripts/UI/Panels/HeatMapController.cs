using System.Linq;
using Simulation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI.Panels
{
    [RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
    public class HeatMapController : MonoBehaviour
    {
        private Mesh agentPositionMesh;

        public RenderTexture AgentPositionRenderTexture;
        
        public Material AgentPositionMaterial;
        private readonly int aspectId = Shader.PropertyToID("_ScreenAspect");
        
        public Material HeatMapColorBandMaterial;
        private readonly int maxDensityId = Shader.PropertyToID("_MaxDensity");

        public MeshRenderer RoadsRenderer;
        public Material RoadsMaterial;

        private CommandBuffer buffer;
        
        private AspectRatioFitter aspectRatioFitter;
        private float previousAspectRatio = 0;

        private void Awake()
        {
            agentPositionMesh = new Mesh();
            agentPositionMesh.MarkDynamic();

            aspectRatioFitter = GetComponent<AspectRatioFitter>();

            var rawImage = GetComponent<RawImage>();
            rawImage.texture = AgentPositionRenderTexture;
            
            // Initialize the command buffer
            buffer = new CommandBuffer {name = "HeatMap Render Buffer"};
            buffer.SetRenderTarget(AgentPositionRenderTexture);
            buffer.ClearRenderTarget(false, true, Color.black);
            buffer.DrawRenderer(RoadsRenderer, RoadsMaterial, 0, 0);
            buffer.DrawMesh(agentPositionMesh, Matrix4x4.identity, AgentPositionMaterial, 0, 0);
            // var previousRenderTarget = cam.targetTexture;
            //buffer.SetRenderTarget(previousRenderTarget);
        }

        private void OnEnable() => RenderPipelineManager.endCameraRendering += HeatMapRendering;
        private void OnDisable() => RenderPipelineManager.endCameraRendering -= HeatMapRendering;
        private void HeatMapRendering(ScriptableRenderContext context, Camera cam)
        {
            if (cam.cameraType != CameraType.Game || cam.name != UIState.Camera.Get().name)
                return;
            
            context.ExecuteCommandBuffer(buffer);
            //context.Submit();
        }

        private void Update()
        {
            var cam = UIState.Camera.Get();
            var agentPositions = Agent.AgentsInStates[AgentState.Walking]
                .Select(agent => agent.transform.position)
                .Where(p =>
                {
                    p = cam.WorldToViewportPoint(p);
                    return 0 < p.x && p.x < 1 && 0 < p.y && p.y < 1;
                })
                .ToArray();

            var indices = Enumerable.Range(0, agentPositions.Length).ToArray();

            agentPositionMesh.Clear();
            agentPositionMesh.SetVertices(agentPositions);
            agentPositionMesh.SetIndices(indices, MeshTopology.Points, 0);
            
            //Debug.Log($"Agents in HM {agentPositionMesh.vertexCount.ToString()}");

            var maxDensity = math.max(agentPositions.Length, 4);
            HeatMapColorBandMaterial.SetFloat(maxDensityId, maxDensity);
            
            var aspect = UIState.Camera.Get().aspect;
            if (previousAspectRatio != aspect)
            {
                AgentPositionMaterial.SetFloat(aspectId, aspect);
                aspectRatioFitter.aspectRatio = aspect;
                previousAspectRatio = aspect;
            }
        }
    }
}
