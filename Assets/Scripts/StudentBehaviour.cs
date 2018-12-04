using System.Collections;
using System.Collections.Generic;
using UMA;
using UnityEngine;
using UnityEngine.AI;

public class StudentBehaviour : MonoBehaviour
{

	public NavMeshAgent agent;
	public Animator animator;
	
	void Start ()
	{
		animator = GetComponent<UMAData>().animator;
		transform.position = new Vector3(Random.Range(-28, 28f), 0, Random.Range(-16, 16f));
		
		NavMeshHit closestHit;
		if (NavMesh.SamplePosition (transform.position, out closestHit, 100f, NavMesh.AllAreas)) {
			transform.position = closestHit.position;
			agent = gameObject.AddComponent<NavMeshAgent>();
			agent.speed = 0.1f;
		}
	}
}
