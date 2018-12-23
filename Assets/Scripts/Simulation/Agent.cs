using UnityEngine;
using UnityEngine.AI;
using World;

namespace Simulation
{
	public class Agent : MonoBehaviour
	{
		private NavMeshAgent agent;

		private void Awake()
		{
			agent = GetComponent<NavMeshAgent>();
			agent.autoBraking = true;
			agent.acceleration = 1000f;
			agent.speed = 10f;
			agent.angularSpeed = 3600f;
		}

		public void SetTarget(Door door)
		{
			NavMeshHit closestHit;
			if (NavMesh.SamplePosition(door.gameObject.transform.position, out closestHit, 100f, NavMesh.AllAreas))
			{
				agent.SetDestination(closestHit.position);
			}
		}

		public void SetStartingPosition(Door door)
		{
			NavMeshHit closestHit;
			if (NavMesh.SamplePosition(door.gameObject.transform.position, out closestHit, 100f, NavMesh.AllAreas))
			{
				transform.position = closestHit.position;
			}
		}
	}
}