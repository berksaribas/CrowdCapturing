using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using World;

namespace Simulation
{
	public class CrowdManager : MonoBehaviour {
		public GameObject agentPrefab;
		
		private List<Agent> agents;

		private void Awake()
		{			
			agents = new List<Agent>();
		}

		public void CreateAgent(Door startingDoor, Door finishingDoor)
		{
			var agent = Instantiate(agentPrefab);
			agent.GetComponent<NavMeshAgent>().enabled = false;
			var agentComponent = agent.AddComponent<Agent>();
			
			agentComponent.SetStartingPosition(startingDoor);
			agentComponent.SetTarget(finishingDoor);
			
			agents.Add(agentComponent);
		}

		private void Update()
		{
			foreach (var agent in agents)
			{
				var navMeshAgent = agent.GetComponent<NavMeshAgent>();
				if (!navMeshAgent.pathPending)
				{
					if (navMeshAgent.remainingDistance <= 1f)
					{
						agent.TargetReached = true;
					}
				}
			}

			var reachedAgents = agents.FindAll(item => item.TargetReached);
			foreach (var reachedAgent in reachedAgents)
			{
				agents.Remove(reachedAgent);
				Destroy(reachedAgent.gameObject);
			}
		}
	}
}