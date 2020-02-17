using Control;
using UnityEngine;

namespace UI.State
{
    public class CursorHider : MonoBehaviour
    {
        public CameraHandler CameraHandler;

        private bool isCursorLocked;
        [SerializeField] public KeyCode ToggleKey = KeyCode.Tab;

        private void Start()
        {
            CameraHandler.Observe(activeCamera => SetMode());

            isCursorLocked = true;
            SetMode();
        }

        private void Update()
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