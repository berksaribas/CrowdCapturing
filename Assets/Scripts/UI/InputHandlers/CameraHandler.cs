using System;
using UnityEngine;

namespace UI.InputHandlers
{
    public class CameraHandler : MonoBehaviour
    {
        [Serializable]
        public struct CameraKeyPair
        {
            public Camera Camera;
            public KeyCode ActivationKey;
        }
        
        public CameraKeyPair[] Cameras;

        private void Awake()
        {
            UIState.Camera.OnChange += SetActiveCamera;
            UIState.Camera.Set(Cameras[0].Camera);
        }

        private void Update()
        {
            for (var i = 0; i < Cameras.Length; i++)
            {
                if (Input.GetKeyDown(Cameras[i].ActivationKey))
                {
                    UIState.Camera.Set(Cameras[i].Camera);
                    break;
                }
            }
        }

        private void SetActiveCamera(Camera activeCamera)
        {
            for (var i = 0; i < Cameras.Length; i++)
            {
                Cameras[i].Camera.gameObject.SetActive(Cameras[i].Camera == activeCamera);
            }
        }
    }
}