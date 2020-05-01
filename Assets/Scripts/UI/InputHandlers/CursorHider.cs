using Control;
using UnityEngine;

namespace UI.InputHandlers
{
    public class CursorHider : MonoBehaviour
    {
        private bool isCursorLocked;
        [SerializeField] public KeyCode ToggleKey = KeyCode.Tab;

        private void Start()
        {
            UIState.Camera.OnChange += newCamera => SetMode();

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
                UIState.Camera.Get().GetComponent<CameraMovement>().enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                UIState.Camera.Get().GetComponent<CameraMovement>().enabled = true;
            }
        }
    }
}