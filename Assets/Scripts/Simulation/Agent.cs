using UnityEngine;
using UnityEngine.AI;
using World;

namespace Simulation
{
	public class Agent : MonoBehaviour
	{
		private NavMeshAgent agent;
		private Door startingDoor, targetDoor;

		private void Awake()
		{
			agent = GetComponent<NavMeshAgent>();
			agent.autoBraking = true;
			agent.acceleration = 1000f;
			agent.speed = Random.Range(6f, 11f);
			agent.angularSpeed = 3600f;
		}

		public void SetTarget(Door door)
		{
			GetComponent<NavMeshAgent>().enabled = true;

			targetDoor = door;
			
			NavMeshHit closestHit;
			if (NavMesh.SamplePosition(door.gameObject.transform.position, out closestHit, 100f, NavMesh.AllAreas))
			{
				agent.SetDestination(closestHit.position);
			}
		}

		public void SetStartingPosition(Door door)
		{
			startingDoor = door;
			
			NavMeshHit closestHit;
			if (NavMesh.SamplePosition(door.gameObject.transform.position, out closestHit, 100f, NavMesh.AllAreas))
			{
				transform.position = closestHit.position;
			}
		}

		public string GetStartingDoorName()
		{
			return startingDoor.gameObject.name;
		}
		
		public string GetTargetDoorName()
		{
			return targetDoor.gameObject.name;
		}
	}
}