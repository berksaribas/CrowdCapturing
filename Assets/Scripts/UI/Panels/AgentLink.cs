using Simulation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    [RequireComponent(typeof(Button))]
    public class AgentLink : MonoBehaviour
    {
        private Agent agent;
        public Agent Agent
        {
            get => agent;
            set
            {
                agent = value;
                
                gameObject.SetActive(agent != null);
            }
        }

        public TextMeshProUGUI ID;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => UIState.Agent.Set(Agent));
        }

        private void OnGUI()
        {
            ID.text = $"A#<mspace=0.6em>{agent.Id.ToString()}</mspace>";
        }
    }
}