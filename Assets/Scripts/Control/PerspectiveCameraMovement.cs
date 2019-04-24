using UnityEngine;

namespace Control
{
    public class PerspectiveCameraMovement : CameraMovement
    {
        public float VerticalAngleLimit = 75.0f;

        protected override Vector3 GetEulerRotation()
        {
            var mouseDelta = GetMouseDelta();
            HorizontalAngle += mouseDelta.x;
            VerticalAngle = Mathf.Clamp(VerticalAngle + mouseDelta.y, -VerticalAngleLimit, VerticalAngleLimit);

            return new Vector3(VerticalAngle, HorizontalAngle, 0.0f);
        }

        protected override Vector3 GetLocalDirection()
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

        protected override Vector3 GetGlobalDirection()
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