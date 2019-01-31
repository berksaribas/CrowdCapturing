using UnityEngine;

namespace Control
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField]
        private float speed = 100.0f;

        [SerializeField]
        private float mouseSensitivity = 0.6f;

        [SerializeField]
        private float verticalAngleLimit = 75.0f;

        private float verticalAngle, horizontalAngle;


        void Start()
        {
            verticalAngle = transform.eulerAngles.x % 360.0f;
            horizontalAngle = transform.eulerAngles.y % 360.0f;
        }

        void Update()
        {
            var mouseDelta = GetMouseDelta() * mouseSensitivity;
            horizontalAngle += mouseDelta.x;
            verticalAngle = Mathf.Clamp(verticalAngle + mouseDelta.y, -verticalAngleLimit, verticalAngleLimit);

            transform.rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0.0f);

            transform.Translate(GetLocalDirectionFromKeys() * speed * Time.deltaTime);
            transform.Translate(GetGlobalDirectionFromKeys() * speed * Time.deltaTime, Space.World);
        }

        private static Vector2 GetMouseDelta()
        {
            return new Vector2(
                Input.GetAxis("Mouse X"),
                -Input.GetAxis("Mouse Y") // Axis Y is inverted for some reason
            );
        }

        private static Vector3 GetLocalDirectionFromKeys()
        {
            var direction = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }

            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }

            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }

            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }

            return direction.normalized;
        }
    
        private static Vector3 GetGlobalDirectionFromKeys()
        {
            var direction = Vector3.zero;

            if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.up;
            }

            if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.down;
            }

            return direction.normalized;
        }
    }
}