using Control;
using Simulation;
using UI.State;
using UnityEngine;

namespace UI
{
    public class AgentHighlighter : MonoBehaviour
    {
        private Agent focusedAgent;

        public MeshRenderer HighlighterObject;

        private readonly PathHighlighter highlighter = new PathHighlighter();
    
        void Start()
        {
            FocusedAgent.Observe(newFocus =>
            {
                highlighter.Disable();

                if ((focusedAgent = newFocus) != null)
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
            });
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
