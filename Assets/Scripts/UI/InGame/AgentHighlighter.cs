using Simulation;
using UnityEngine;

namespace UI.InGame
{
    public class AgentHighlighter : MonoBehaviour
    {
        private Agent focusedAgent;

        public MeshRenderer HighlighterObject;

        void Start()
        {
            UIState.Agent.OnChange += newAgent =>
            {
                if ((focusedAgent = newAgent) != null)
                {
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
    }
}
