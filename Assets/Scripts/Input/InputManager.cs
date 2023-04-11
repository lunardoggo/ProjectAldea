using UnityEngine;
using System;

namespace ProjectAldea.Scripts
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [SerializeField]
        private float panSpeed = 0.1f;
        [SerializeField]
        private float minPanDistance = 0.25f;
        [SerializeField]
        private float moveSpeed = 0.75f;
        [SerializeField]
        private float zoomSpeed = 1.0f;
        [SerializeField]
        private bool invertZoom = false;

        private Vector2? panStart = null;

        private void Start()
        {
            if (InputManager.Instance != null)
            {
                throw new InvalidOperationException("Instance of InputManager already exists");
            }
            InputManager.Instance = this;
        }

        public Vector2 MoveInput { get; private set; }
        public float ZoomInput { get; private set; }

        private void Update()
        {
            Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * this.moveSpeed;

            if (Input.GetMouseButtonDown(1))
            {
                this.panStart = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                this.panStart = null;
            }

            if (this.panStart.HasValue)
            {
                Vector2 move = ((Vector2)Input.mousePosition - this.panStart.Value);
                if(move.magnitude > this.minPanDistance)
                {
                    movement = move * this.panSpeed;
                }
            }

            this.MoveInput = movement;
            this.ZoomInput = (this.invertZoom ? 1 : -1) * Input.GetAxis("Mouse ScrollWheel") * this.zoomSpeed;
        }
    }
}
