using System;
using UnityEngine;

namespace UI.State
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
            FocusedCamera.Observe(SetActiveCamera);
            FocusedCamera.Set(Cameras[0].Camera);
        }

        private void Update()
        {
            for (var i = 0; i < Cameras.Length; i++)
            {
                if (Input.GetKeyDown(Cameras[i].ActivationKey))
                {
                    FocusedCamera.Set(Cameras[i].Camera);
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