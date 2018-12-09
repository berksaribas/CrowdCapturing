using System.Collections;
using System.Collections.Generic;
using UMA;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class StudentBehaviour : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;

    void Start()
    {
        animator = GetComponent<UMAData>().animator;
        transform.position = new Vector3(Random.Range(-200f, 100f), 0, Random.Range(-200f, 300f));

        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(transform.position, out closestHit, 100f, NavMesh.AllAreas))
        {
            transform.position = closestHit.position;
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.autoBraking = true;
            agent.acceleration = 1000f;
            agent.speed = 10f;
            agent.angularSpeed = 3600f;
        }
    }

    private void Update()
    {
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
        Debug.Log(agent.velocity.sqrMagnitude);

        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            //transform.localScale = Vector3.one * 15F;
        }
        
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            //transform.localScale = Vector3.one * 30F;
        }

        if (Selection.Contains(gameObject) && agent.hasPath)
        {
//            var Cylinder = Resources.Load("Cylinder") as GameObject;
//            var path = new List<GameObject>();
//            
//            for (int i = 0; i < agent.path.corners.Length; i++)
//            {
//                
//                var c = Instantiate(Cylinder);
//                path.Add(c);
//            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.75F);
        Gizmos.DrawSphere(transform.position, 3f);
    }
}