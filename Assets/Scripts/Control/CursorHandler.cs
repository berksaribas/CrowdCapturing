using UnityEngine;

namespace Control
{
    public class CursorHandler : MonoBehaviour
    {
        [SerializeField] public KeyCode ToggleKey = KeyCode.Tab;

        public CameraHandler CameraHandler;

        private bool isCursorLocked;

        void Start()
        {
            CameraHandler.Observe(activeCamera =>
            {
                SetMode();
            });

            isCursorLocked = true;
            SetMode();
        }

        void Update()
        {
            if (Input.GetKeyDown(ToggleKey))
            {
                isCursorLocked = !isCursorLocked;

                SetMode();
            }
        }

        private void SetMode()
        {
            if (isCursorLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                CameraHandler.ActiveCamera.GetComponent<CameraMovement>().enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                CameraHandler.ActiveCamera.GetComponent<CameraMovement>().enabled = true;
            }
        }
    }
}