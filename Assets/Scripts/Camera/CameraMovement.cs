using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectAldea.Scripts
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField]
        private float smoothTime;
        [SerializeField]
        private float moveSpeed;

        private Vector3 lastVelocity;

        private void Update()
        {
            if(InputManager.Instance.MoveInput.magnitude != 0)
            {
                Vector3 input = new Vector3(InputManager.Instance.MoveInput.x, 0, InputManager.Instance.MoveInput.y);

                this.transform.position = Vector3.SmoothDamp(this.transform.position, this.transform.position + input * this.moveSpeed, ref this.lastVelocity, Time.unscaledDeltaTime * this.smoothTime);
            }
        }
    }
}
