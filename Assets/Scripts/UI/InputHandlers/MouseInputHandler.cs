using Simulation;
using UnityEngine;
using UnityEngine.EventSystems;
using World;

namespace UI.InputHandlers
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

            var ray = UIState.Camera.Get().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, agentLayer))
            {
                var hitObject = hit.collider.gameObject;
                var hitLayer = 1 << hitObject.layer;

                if (hitLayer == agentLayer)
                {
                    UIState.Agent.Set(hit.transform.GetComponent<Agent>());
                    UIState.Group.Set(GroupManager.GetActiveGroup(UIState.Agent.Get()));
                }
            }
            else
            {
                UIState.Agent.Set(null);
                UIState.Group.Set(null);
            }
        }

        private void HandleRightClick()
        {
            if (!Input.GetMouseButtonDown(1) || EventSystem.current.IsPointerOverGameObject())
                return;

            var ray = UIState.Camera.Get().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, buildingLayer))
            {
                var hitObject = hit.collider.gameObject;
                var hitLayer = 1 << hitObject.layer;

                if (hitLayer == buildingLayer)
                {
                    UIState.Building.Set(hitObject.GetComponent<Building>());
                }
            }
            else
            {
                UIState.Building.Set(null);
            }
        }
    }
}