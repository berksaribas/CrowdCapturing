using Control;
using UnityEngine;

namespace UI.State
{
    public class CursorHider : MonoBehaviour
    {
        private bool isCursorLocked;
        [SerializeField] public KeyCode ToggleKey = KeyCode.Tab;

        private void Start()
        {
            FocusedCamera.Observe(newFocus => SetMode());

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
                FocusedCamera.Get().GetComponent<CameraMovement>().enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                FocusedCamera.Get().GetComponent<CameraMovement>().enabled = true;
            }
        }
    }
}