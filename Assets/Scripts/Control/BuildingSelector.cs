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

            UpdateCanvas();
        }

        private void Update()
        {
            UpdateCanvas();

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
            }
            else
            {
                FocusedBuilding = null;
                highlightBox.enabled = false;
            }
        }

        private void UpdateCanvas()
        {
            if (FocusedBuilding != null)
            {
                canvas.GetChild(0).GetComponent<Text>().text = FocusedBuilding.gameObject.name;
            }
            else
            {
                canvas.GetChild(0).GetComponent<Text>().text = "Select A Building";
            }
        }
    }
}