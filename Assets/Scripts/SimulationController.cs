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

    [Range(0, 1000)] public int CrowdSize;

    private List<GameObject> agents = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        var go = crowd.GenerateUMA(Random.Range(0, 2), Vector3.zero);
        UMAData umaData = go.GetComponent<UMAData>();
        umaData.CharacterCreated.AddListener(CharacterCreated);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Giving all agents a random destination");
            foreach (var agent in agents)
            {
                var navMesh = agent.GetComponent<NavMeshAgent>();
                if (navMesh != null)
                {
                    NavMeshHit closestHit;
                    var randomPoint = new Vector3(Random.Range(-200f, 100f), 0, Random.Range(-200f, 300f));

                    while (!NavMesh.SamplePosition(randomPoint, out closestHit, 100f, NavMesh.AllAreas))
                    {
                        randomPoint = new Vector3(Random.Range(-200f, 100f), 0, Random.Range(-200f, 300f));
                    }
                
                    navMesh.SetDestination(closestHit.position);
                }
            }
        }
    }

    void CharacterCreated(UMAData umaData)
    {
        umaData.transform.localScale = new Vector3(1f, 1f, 1f);
        umaData.gameObject.AddComponent<StudentBehaviour>();
        foreach (var componentsInChild in umaData.gameObject.GetComponentsInChildren<Renderer>())
        {
            foreach (var material in componentsInChild.materials)
            {
                material.enableInstancing = true;
            }
        }

        for (int i = 0; i < CrowdSize; i++)
        {
            crowd.ResetSpawnPos();

            NavMeshHit closestHit;
            var randomPoint = new Vector3(Random.Range(-200f, 100f), 0, Random.Range(-200f, 300f));

            while (!NavMesh.SamplePosition(randomPoint, out closestHit, 100f, NavMesh.AllAreas))
            {
                randomPoint = new Vector3(Random.Range(-200f, 100f), 0, Random.Range(-200f, 300f));
            }

            var clone_umaData = Instantiate(umaData.gameObject);

            clone_umaData.transform.position = closestHit.position;
            clone_umaData.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            clone_umaData.transform.parent = umaData.transform.parent;

            var lod = clone_umaData.AddComponent<O3nUMASimpleLOD>();
            lod.lodDistance = 4;
            lod.swapSlots = true;
            lod.lodOffset = 0;
            lod.Update();

            agents.Add(clone_umaData);

            Debug.Log(i);
        }
    }
}