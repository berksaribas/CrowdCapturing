using UnityEngine;

namespace Control
{
    public abstract class CameraMovement : MonoBehaviour
    {
        public float Speed = 100.0f;
        public float MouseSensitivity = 0.6f;
        public float VerticalAngle, HorizontalAngle;

        protected void Start()
        {
            VerticalAngle = transform.eulerAngles.x % 360.0f;
            HorizontalAngle = transform.eulerAngles.y % 360.0f;
        }

        protected void Update()
        {
            transform.rotation = Quaternion.Euler(GetEulerRotation());
            transform.Translate(GetLocalDirection() * Speed * Time.deltaTime);
            transform.Translate(GetGlobalDirection() * Speed * Time.deltaTime, Space.World);
        }

        protected Vector2 GetMouseDelta()
        {
            return new Vector2(
                Input.GetAxis("Mouse X") * MouseSensitivity,
                -Input.GetAxis("Mouse Y") * MouseSensitivity // Axis Y is inverted for some reason
            );
        }

        protected abstract Vector3 GetEulerRotation();

        protected abstract Vector3 GetLocalDirection();

        protected abstract Vector3 GetGlobalDirection();
    }
}