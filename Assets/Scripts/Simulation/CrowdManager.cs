using System.Collections.Generic;
using UnityEngine;
using World;

namespace Simulation
{
	public class CrowdManager : MonoBehaviour {
		public GameObject agentPrefab;

		private static CrowdManager _instance;

		public static CrowdManager Instance { get { return _instance; } }

		private List<Agent> agents;

		private void Awake()
		{
			if (_instance != null && _instance != this)
			{
				Destroy(this.gameObject);
			} else {
				_instance = this;
			}
			
			agents = new List<Agent>();
		}

		public void CreateAgent(Door startingDoor, Door finishingDoor)
		{
			var agent = Instantiate(agentPrefab);
			var agentComponent = agent.AddComponent<Agent>();
			
			agentComponent.SetStartingPosition(startingDoor);
			agentComponent.SetStartingPosition(finishingDoor);
		}
	}
}