using Simulation;
using UnityEngine;

namespace UI.InGame
{
    public class AgentHighlighter : MonoBehaviour
    {
        private Agent focusedAgent;

        public MeshRenderer HighlighterObject;

        private readonly PathHighlighter highlighter = new PathHighlighter();
    
        void Start()
        {
            UIState.Agent.OnChange += newAgent =>
            {
                highlighter.Disable();

                if ((focusedAgent = newAgent) != null)
                {
                    highlighter.Initialize(focusedAgent, transform);
                    
                    HighlighterObject.enabled = true;
                    HighlighterObject.transform.SetParent(focusedAgent.transform, false);
                }
                else
                {
                    HighlighterObject.enabled = false;
                    HighlighterObject.transform.SetParent(transform, false);
                }
            };
        }

        private void Update()
        {
            if (focusedAgent != null)
            {
                highlighter.Update();
            }
        }
    }
}
