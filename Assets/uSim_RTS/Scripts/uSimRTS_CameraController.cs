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
        [Tooltip("Pointer travel in pixels required before a right click becomes a camera-rotation drag.")]
        [Min(0f)] public float rotationDragThreshold = 8f;
        public Transform worldCamRoot;
        public bool rotating;
        public bool WasRotationDragThisGesture { get; private set; }

        Vector2 rotationPointerStart;
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
            Vector2 mousePosition = mouse.position.ReadValue();

            if (mouse.rightButton.wasPressedThisFrame)
            {
                rotationPointerStart = mousePosition;
                rotating = false;
                WasRotationDragThisGesture = false;
            }

            if (mouse.rightButton.isPressed || mouse.rightButton.wasReleasedThisFrame)
            {
                if (!WasRotationDragThisGesture &&
                    Vector2.Distance(rotationPointerStart, mousePosition) >= rotationDragThreshold)
                {
                    WasRotationDragThisGesture = true;
                }

                if (WasRotationDragThisGesture && mouse.rightButton.isPressed)
                {
                    rotating = true;
                    float rotationAmount = mouseDelta.x * Time.deltaTime * rotSpeed;
                    Vector3 rotation = worldCamRoot.rotation.eulerAngles;
                    rotation.y += rotationAmount;

                    worldCamRoot.rotation = Quaternion.Euler(rotation);
                }
            }

            if (mouse.rightButton.wasReleasedThisFrame)
                rotating = false;
        }

        void ScrollWindow (Vector3 movement)
        {            
            worldCamRoot.Translate(movement);
        }
    }

}
