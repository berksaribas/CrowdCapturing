using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.State
{
    public class CameraHandler : MonoBehaviour
    {
        [System.Serializable]
        public struct CameraKeyPair
        {
            public Camera Camera;
            public KeyCode ActivationKey;
        }
        
        public CameraKeyPair[] Cameras;

        private int activeCameraIndex;
        private readonly List<Action<Camera>> listeners = new List<Action<Camera>>();

        public int ActiveCameraIndex
        {
            get => activeCameraIndex;
            set
            {
                activeCameraIndex = value;
                for (var i = 0; i < Cameras.Length; i++)
                {
                    Cameras[i].Camera.gameObject.SetActive(i == activeCameraIndex);
                }
                
                listeners.ForEach(action => action.Invoke(ActiveCamera));
            }
        }

        public Camera ActiveCamera => Cameras[activeCameraIndex].Camera;

        private void Start()
        {
            ActiveCameraIndex = 0;
        }

        private void Update()
        {
            for (var i = 0; i < Cameras.Length; i++)
            {
                if (Input.GetKeyDown(Cameras[i].ActivationKey))
                {
                    ActiveCameraIndex = i;
                    break;
                }
            }
        }

        public void Observe(Action<Camera> listener)
        {
            listeners.Add(listener);
        }
    }
}