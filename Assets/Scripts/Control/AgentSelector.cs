using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Simulation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Util;

namespace Control
{
    public class AgentSelector : MonoBehaviour
    {
        [CanBeNull] [NonSerialized] public Agent FocusedAgent = null;

        public GameObject CanvasObject;
        private RectTransform canvas;

        public GameObject AgentHighlighter;
        private MeshRenderer agentHighlighter;

        public GameObject PathHighlighterPrefab;
        private readonly List<GameObject> pathHighlighter = new List<GameObject>();

        private void Awake()
        {
            agentHighlighter = AgentHighlighter.GetComponent<MeshRenderer>();
            canvas = CanvasObject.GetComponent<RectTransform>();

            ResetCanvas();
        }

        private void Update()
        {
            if (FocusedAgent != null)
            {
                AgentHighlighter.transform.position = FocusedAgent.transform.position;
                UpdateCanvas();
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

                FocusedAgent = hit.transform.gameObject.GetComponent<Agent>();

                agentHighlighter.enabled = true;
                ConfigurePathHighlighterTo(FocusedAgent);

                SetCanvas();
            }
            else
            {
                FocusedAgent = null;
                agentHighlighter.enabled = false;
                ClearPathHighlighter();

                ResetCanvas();
            }
        }

        private void ResetCanvas()
        {
            canvas.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            canvas.GetChild(0).GetComponent<Text>().text = "Select An Agent";
        }

        private void SetCanvas()
        {
            canvas.GetChild(0).GetComponent<Text>().alignment = TextAnchor.UpperLeft;
            canvas.GetChild(0).GetComponent<Text>().text = String.Join(
                "\n",
                new[]
                {
                    "From:",
                    $"{FocusedAgent.GetStartingDoorName()} @ {TimeHelper.ConvertSecondsToString(FocusedAgent.GetNextSequence().StartTime)}",
                    "",
                    "To:",
                    $"{FocusedAgent.GetTargetDoorName()}",
                }
            );
        }

        private void UpdateCanvas()
        {
            
        }

        private void ClearPathHighlighter()
        {
            pathHighlighter.ForEach(Destroy);
            pathHighlighter.Clear();
        }

        private void ConfigurePathHighlighterTo(Agent agent)
        {
            var path = agent.GetComponent<NavMeshAgent>().path.corners;

            for (var i = 1; i < path.Length; i++)
            {
                var start = path[i - 1];
                var end = path[i];

                var difference = start - end;

                var center = start - difference / 2f;
                var rotation = Quaternion.Euler(90f, Mathf.Rad2Deg * Mathf.Atan2(difference.x, difference.z), 0);

                var highlighter = Instantiate(PathHighlighterPrefab, center, rotation, transform);
                highlighter.transform.localScale = new Vector3(1.0f, difference.magnitude / 2f, 1.0f);

                pathHighlighter.Add(highlighter);
            }
        }
    }
}