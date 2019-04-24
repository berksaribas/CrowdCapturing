using UnityEngine;

namespace Control
{
    public class OrthographicCameraMovement : CameraMovement
    {
        private new void Update()
        {
            base.Update();
            
            if (Input.GetKey(KeyCode.Q))
            {
                GetComponent<Camera>().orthographicSize += 1.0f;
            }

            if (Input.GetKey(KeyCode.E))
            {
                GetComponent<Camera>().orthographicSize -= 1.0f;
            }
        }

        protected override Vector3 GetEulerRotation()
        {
            var mouseDelta = GetMouseDelta();
            HorizontalAngle += mouseDelta.x;

            return new Vector3(VerticalAngle, HorizontalAngle, 0.0f);
        }

        protected override Vector3 GetLocalDirection()
        {
            var direction = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.up;
            }

            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.down;
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

            return direction.normalized;
        }
    }
}