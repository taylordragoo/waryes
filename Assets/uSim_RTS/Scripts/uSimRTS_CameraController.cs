using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace uSimRTS
{
    public class uSimRTS_CameraController : MonoBehaviour
    {
        [Tooltip("Camera movement speed.")]
        public float scrollSpeed = 3f;
        [Tooltip("Camera zoom in/out sensitivity.")]
        public float zoomSpeed = 100f;
        [Tooltip("Camera rotation sensitivity.")]
        public float rotSpeed = 10f;
        public Transform worldCamRoot;
        public bool rotating;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            Camera mainCamera = Camera.main;

            if (keyboard != null)
            {
                Vector3 movement = Vector3.zero;

                if (keyboard.wKey.isPressed)
                    movement += Vector3.forward;
                if (keyboard.sKey.isPressed)
                    movement += Vector3.back;
                if (keyboard.dKey.isPressed)
                    movement += Vector3.right;
                if (keyboard.aKey.isPressed)
                    movement += Vector3.left;

                if (movement.sqrMagnitude > 0f)
                    ScrollWindow(movement.normalized * scrollSpeed * Time.deltaTime);
            }

            if (mouse == null || mainCamera == null)
                return;

            Vector2 mouseDelta = mouse.delta.ReadValue() * 0.1f;
            float scrollDelta = mouse.scroll.ReadValue().y / 1200f;

            mainCamera.fieldOfView -= scrollDelta * zoomSpeed;

            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 35f, 60f);

            if (mouse.middleButton.isPressed)
            {
                Vector3 mov = new Vector3(-mouseDelta.x * Time.deltaTime * scrollSpeed, 0f, -mouseDelta.y * Time.deltaTime * scrollSpeed);
                ScrollWindow(mov);
            }
            if (mouse.rightButton.isPressed)
            {
                rotating = false;
                float rotS = mouseDelta.x * Time.deltaTime * rotSpeed;
                if (rotS > 0.01f || rotS < -0.01f)
                    rotating = true;

                Vector3 rot = worldCamRoot.rotation.eulerAngles;
                rot.y += rotS;

                worldCamRoot.rotation = Quaternion.Euler(rot);
            }
        }

        void ScrollWindow (Vector3 movement)
        {            
            worldCamRoot.Translate(movement);
        }
    }

}
