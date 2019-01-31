using UnityEngine;

namespace Control
{
    public class CursorHandler : MonoBehaviour
    {
        [SerializeField] public KeyCode ToggleKey = KeyCode.Tab;

        private bool isCursorLocked;

        void Start()
        {
            isCursorLocked = false;
            ToggleCursorMode();
        }

        void Update()
        {
            if (Input.GetKeyDown(ToggleKey))
            {
                ToggleCursorMode();
            }
        }

        private void ToggleCursorMode()
        {
            if (isCursorLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                gameObject.GetComponent<CameraMovement>().enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                gameObject.GetComponent<CameraMovement>().enabled = true;
            }

            isCursorLocked = !isCursorLocked;
        }
    }
}