using System.Collections;
using System.Collections.Generic;
using UMA;
using UMA.Examples;
using UnityEngine;
using UnityEngine.AI;

public class SimulationController : MonoBehaviour
{
	public GameObject studentPrefab;
	public UMACrowd crowd;
	
	private List<GameObject> agents = new List<GameObject>();
	// Use this for initialization
	void Start ()
	{
		for(int i = 0; i <100; i++)
		{
			crowd.ResetSpawnPos();
			var go = crowd.GenerateUMA(Random.Range(0, 2), transform.position);
			go.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

			UMAData umaData = go.GetComponent<UMAData>();
			umaData.CharacterCreated.AddListener(CharacterCreated);

			var lod = go.AddComponent<O3nUMASimpleLOD>();
			lod.lodDistance = 4;
			lod.swapSlots = true;
			lod.lodOffset = 0;
			lod.Update();
			
			agents.Add(go);
		}
	}
	
	void Update () {
		if(Input.GetMouseButtonDown(0))
		{
			Debug.Log("Giving all agents a random destination");
			foreach (var agent in agents)
			{
				var navMesh = agent.GetComponent<NavMeshAgent>();
				var animator = agent.GetComponent<Animator>();
				if (navMesh != null)
				{
					NavMeshHit closestHit;
					if (NavMesh.SamplePosition(new Vector3(Random.Range(-30f, 30f), 0, Random.Range(-20f, 30f)),
						out closestHit, 100f, NavMesh.AllAreas))
					{
						navMesh.SetDestination(closestHit.position);
					}

					animator.SetFloat("Speed", 0.11f);
				}
			}
		}
	}
	
	void CharacterCreated(UMAData umaData)
	{
		umaData.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		umaData.gameObject.AddComponent<StudentBehaviour>();
	}
}
