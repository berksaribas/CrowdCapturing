using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using World;

namespace Control
{
    public class BuildingSelector : MonoBehaviour
    {
        [CanBeNull] [NonSerialized] public Building FocusedBuilding = null;

        public GameObject CanvasObject;
        private RectTransform canvas;

        public GameObject HighlighterObject;
        private MeshRenderer highlightBox;

        private void Awake()
        {
            canvas = CanvasObject.GetComponent<RectTransform>();
            highlightBox = HighlighterObject.GetComponent<MeshRenderer>();

            ResetCanvas();
        }

        private void Update()
        {
            if (FocusedBuilding != null)
            {
                UpdateCanvas();
            }

            if (!Input.GetMouseButtonDown(0))
                return;

            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask(new string[] {"Buildings"})))
            {
                FocusedBuilding = hit.transform.gameObject.GetComponent<Building>();

                highlightBox.enabled = true;
                highlightBox.transform.position = FocusedBuilding.transform.position;
                highlightBox.transform.rotation = FocusedBuilding.transform.rotation;
                highlightBox.transform.localScale = FocusedBuilding.GetComponent<BoxCollider>().size;

                SetCanvas();
            }
            else
            {
                FocusedBuilding = null;
                highlightBox.enabled = false;

                ResetCanvas();
            }
        }

        private void ResetCanvas()
        {
            canvas.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            canvas.GetChild(0).GetComponent<Text>().text = "Select A Building";
        }

        private void SetCanvas()
        {
            canvas.GetChild(0).GetComponent<Text>().alignment = TextAnchor.UpperLeft;
        }

        private void UpdateCanvas()
        {
            canvas.GetChild(0).GetComponent<Text>().text = String.Join(
                "\n",
                new[]
                {
                    $"{FocusedBuilding.gameObject.name}",
                    "",
                    $"Has {FocusedBuilding.AgentCount.ToString()} agents inside.",
                }
            );
        }
    }
}