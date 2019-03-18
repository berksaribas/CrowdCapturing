using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace Control
{
    public class AgentSelector : MonoBehaviour
    {
        [CanBeNull] [NonSerialized] public GameObject FocusedAgent = null;
        
        public GameObject ActorHighlighterPrefab;
        private GameObject agentHighlighter;
        
        public GameObject PathHighlighterPrefab;
        private readonly List<GameObject> pathHighlighter = new List<GameObject>();

        private void Awake()
        {
            agentHighlighter = Instantiate(ActorHighlighterPrefab);
        }

        private void Update()
        {
            if (FocusedAgent != null)
            {
                agentHighlighter.transform.position = FocusedAgent.transform.position;
            }
            
            if (!Input.GetMouseButtonDown(0))
                return;

            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask(new string[] {"Agents"})))
            {
                if (FocusedAgent != null)
                {
                    ClearPathHighlighter();
                }
                
                FocusedAgent = hit.transform.gameObject;
                
                agentHighlighter.GetComponent<MeshRenderer>().enabled = true;
                ConfigurePathHighlighterTo(FocusedAgent);
            }
            else
            {
                agentHighlighter.GetComponent<MeshRenderer>().enabled = false;
                ClearPathHighlighter();
            }
        }

        private void ClearPathHighlighter()
        {
            pathHighlighter.ForEach(Destroy);
            pathHighlighter.Clear();
        }

        private void ConfigurePathHighlighterTo(GameObject agent)
        {
            var path = agent.GetComponent<NavMeshAgent>().path.corners;
                
            for (var i = 1; i < path.Length; i++)
            {
                var start = path[i - 1];
                var end = path[i];
                
                var difference = start - end;
                
                var center = start - difference / 2f;
                var rotation = Quaternion.Euler(90f,  Mathf.Rad2Deg * Mathf.Atan2(difference.x, difference.z), 0);

                var highlighter = Instantiate(PathHighlighterPrefab, center, rotation, transform);
                highlighter.transform.localScale = new Vector3(1.0f, difference.magnitude / 2f, 1.0f);
                    
                pathHighlighter.Add(highlighter);
            }
        }
    }
}