using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace ProjectAldea.Scripts
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField]
        private float smoothTime;
        [SerializeField]
        private Vector2 heightRange;
        [SerializeField]
        private Vector2 angleRange;
        [SerializeField]
        private float minHeightAboveGround;
        [SerializeField]
        private LayerMask terrainLayers;

        private Vector3 lastVelocity;
        private float targetHeight;

        private void Awake()
        {
            this.targetHeight = this.transform.position.y;
        }

        private void Update()
        {
            this.UpdateMovement();
        }

        private void UpdateMovement()
        {
            if(InputManager.Instance.MoveInput.magnitude != 0 || InputManager.Instance.ZoomInput != 0.0f)
            {
                Vector3 position = this.transform.position;
                if(InputManager.Instance.ZoomInput != 0.0f)
                {
                    this.targetHeight = position.y + InputManager.Instance.ZoomInput;
                }

                Vector3 input = new Vector3(InputManager.Instance.MoveInput.x, this.targetHeight - position.y, InputManager.Instance.MoveInput.y);

                position = Vector3.SmoothDamp(position, position + input, ref this.lastVelocity, Time.unscaledDeltaTime * this.smoothTime);
                position = new Vector3(position.x, Mathf.Clamp(position.y, this.heightRange.x, this.heightRange.y), position.z);

                const float origin = 100.0f;
                if(   Physics.Raycast(position + new Vector3(0, origin, 0), Vector3.down, out RaycastHit hit, this.minHeightAboveGround + 2 * origin, this.terrainLayers)
                   && (hit.point - position).magnitude < this.minHeightAboveGround)
                {
                    position = new Vector3(position.x, Mathf.Max(this.targetHeight, hit.point.y + this.minHeightAboveGround), position.z);
                }

                this.transform.position = position;
                this.UpdateCameraTilt();
            }
        }
    
        private void UpdateCameraTilt()
        {
            float angle = Mathf.Pow(this.transform.position.y - this.heightRange.x, 0.8f) + this.angleRange.x;
            Vector3 goal = new Vector3(Mathf.Clamp(angle, this.angleRange.x, this.angleRange.y), 0, 0);
            this.transform.rotation = Quaternion.Euler(goal);
        }
    }
}
