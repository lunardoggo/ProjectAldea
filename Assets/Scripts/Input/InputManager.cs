using UnityEngine;
using System;

namespace ProjectAldea.Scripts
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private void Start()
        {
            if (InputManager.Instance != null)
            {
                throw new InvalidOperationException("Instance of InputManager already exists");
            }
            InputManager.Instance = this;
        }

        public Vector2 MoveInput { get; private set; }

        private void Update()
        {
            this.MoveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }
}
