using Simulation;
using UnityEngine;
using UnityEngine.EventSystems;
using World;

namespace UI.State
{
    public class MouseInputHandler : MonoBehaviour
    {
        public CameraHandler CameraHandler;
        public GroupManager GroupManager;

        private int agentLayer;
        private int buildingLayer;

        private void Awake()
        {
            agentLayer = LayerMask.GetMask("Agents");
            buildingLayer = LayerMask.GetMask("Buildings");
        }

        private void Update()
        {
            HandleLeftClick();
            HandleRightClick();
        }

        private void HandleLeftClick()
        {
            if (!Input.GetMouseButtonDown(0) || EventSystem.current.IsPointerOverGameObject())
                return;

            var ray = FocusedCamera.Get().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, agentLayer))
            {
                var hitObject = hit.collider.gameObject;
                var hitLayer = 1 << hitObject.layer;

                if (hitLayer == agentLayer)
                {
                    FocusedAgent.Set(hit.transform.GetComponent<Agent>());
                    FocusedGroup.Set(GroupManager.GetActiveGroup(FocusedAgent.Get()));
                }
            }
            else
            {
                FocusedAgent.Set(null);
                FocusedGroup.Set(null);
            }
        }

        private void HandleRightClick()
        {
            if (!Input.GetMouseButtonDown(1) || EventSystem.current.IsPointerOverGameObject())
                return;

            var ray = FocusedCamera.Get().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, buildingLayer))
            {
                var hitObject = hit.collider.gameObject;
                var hitLayer = 1 << hitObject.layer;

                if (hitLayer == buildingLayer)
                {
                    FocusedBuilding.Set(hitObject.GetComponent<Building>());
                }
            }
            else
            {
                FocusedBuilding.Set(null);
            }
        }
    }
}