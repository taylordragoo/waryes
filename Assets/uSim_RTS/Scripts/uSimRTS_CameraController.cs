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

        [Header("Zoom")]
        [Tooltip("Camera zoom sensitivity. A value of 100 changes the field of view by five degrees per mouse-wheel step.")]
        public float zoomSpeed = 100f;
        [Tooltip("World-space camera height changed per normalized mouse-wheel step.")]
        [Min(0f)] public float zoomHeightPerStep = 100f;
        [Tooltip("How quickly camera height and field of view ease toward the requested zoom level.")]
        [Min(0.01f)] public float zoomLerpSpeed = 8f;
        [Tooltip("Lowest world-space height allowed while zooming in.")]
        public float minimumCameraHeight = 80f;
        [Tooltip("Highest strategic camera height allowed while zooming out.")]
        public float maximumCameraHeight = 1200f;
        [Tooltip("Closest camera field of view.")]
        [Range(1f, 179f)] public float minimumFieldOfView = 25f;
        [Tooltip("Widest strategic camera field of view.")]
        [Range(1f, 179f)] public float maximumFieldOfView = 85f;

        [Header("Rotation")]
        [Tooltip("Camera rotation sensitivity.")]
        public float rotSpeed = 10f;
        [Tooltip("Pointer travel in pixels required before a right click becomes a camera-rotation drag.")]
        [Min(0f)] public float rotationDragThreshold = 8f;

        [Header("Map Bounds")]
        [Tooltip("Prevent the camera rig from moving beyond the map footprint.")]
        public bool constrainToMapBounds = true;
        [Tooltip("Build movement bounds from all active Terrain objects when play begins.")]
        public bool autoDetectMapBounds = true;
        [Tooltip("Fallback minimum X/Z map coordinates when automatic Terrain detection is disabled or unavailable.")]
        public Vector2 mapBoundsMinimum = Vector2.zero;
        [Tooltip("Fallback maximum X/Z map coordinates when automatic Terrain detection is disabled or unavailable.")]
        public Vector2 mapBoundsMaximum = new Vector2(2000f, 2000f);
        [Tooltip("Distance kept inside each map edge.")]
        [Min(0f)] public float mapBoundsPadding;

        public Transform worldCamRoot;
        public bool rotating;
        public bool WasRotationDragThisGesture { get; private set; }

        Vector2 rotationPointerStart;
        Vector2 activeMapBoundsMinimum;
        Vector2 activeMapBoundsMaximum;
        bool hasActiveMapBounds;
        float targetFieldOfView;
        float targetCameraHeight;
        bool zoomTargetsInitialized;
        // Start is called before the first frame update
        void Start()
        {
            RefreshMapBounds();
            ClampCameraToMapBounds();
            InitializeZoomTargets(Camera.main);
        }

        // Handle camera movement, zoom, and rotation input.
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

            if (mainCamera == null)
                return;

            if (!zoomTargetsInitialized)
                InitializeZoomTargets(mainCamera);

            if (mouse == null)
            {
                UpdateSmoothZoom(mainCamera);
                return;
            }

            Vector2 mouseDelta = mouse.delta.ReadValue() * 0.1f;
            float scrollDelta = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scrollDelta) >= 0.001f)
                UpdateZoomTargets(scrollDelta);

            UpdateSmoothZoom(mainCamera);

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
            ClampCameraToMapBounds();
        }

        void RefreshMapBounds()
        {
            activeMapBoundsMinimum = mapBoundsMinimum;
            activeMapBoundsMaximum = mapBoundsMaximum;
            hasActiveMapBounds = true;

            if (!autoDetectMapBounds)
                return;

            Terrain[] terrains = Terrain.activeTerrains;
            if (terrains == null || terrains.Length == 0)
                return;

            bool foundTerrain = false;
            Vector2 detectedMinimum = Vector2.zero;
            Vector2 detectedMaximum = Vector2.zero;

            for (int i = 0; i < terrains.Length; i++)
            {
                Terrain terrain = terrains[i];
                if (terrain == null || terrain.terrainData == null)
                    continue;

                Vector3 terrainPosition = terrain.transform.position;
                Vector3 terrainSize = terrain.terrainData.size;
                Vector2 terrainMinimum = new Vector2(terrainPosition.x, terrainPosition.z);
                Vector2 terrainMaximum = terrainMinimum + new Vector2(terrainSize.x, terrainSize.z);

                if (!foundTerrain)
                {
                    detectedMinimum = terrainMinimum;
                    detectedMaximum = terrainMaximum;
                    foundTerrain = true;
                    continue;
                }

                detectedMinimum = Vector2.Min(detectedMinimum, terrainMinimum);
                detectedMaximum = Vector2.Max(detectedMaximum, terrainMaximum);
            }

            if (!foundTerrain)
                return;

            activeMapBoundsMinimum = detectedMinimum;
            activeMapBoundsMaximum = detectedMaximum;
        }

        void ClampCameraToMapBounds()
        {
            if (!constrainToMapBounds || !hasActiveMapBounds || worldCamRoot == null)
                return;

            worldCamRoot.position = ClampPositionToBounds(
                worldCamRoot.position,
                activeMapBoundsMinimum,
                activeMapBoundsMaximum,
                mapBoundsPadding);
        }

        void InitializeZoomTargets(Camera mainCamera)
        {
            if (mainCamera != null)
                targetFieldOfView = mainCamera.fieldOfView;

            if (worldCamRoot != null)
                targetCameraHeight = worldCamRoot.position.y;

            zoomTargetsInitialized = mainCamera != null && worldCamRoot != null;
        }

        void UpdateZoomTargets(float rawScrollDelta)
        {
            targetFieldOfView = CalculateZoomFieldOfView(
                targetFieldOfView,
                rawScrollDelta,
                zoomSpeed,
                minimumFieldOfView,
                maximumFieldOfView);

            targetCameraHeight = CalculateZoomHeight(
                targetCameraHeight,
                rawScrollDelta,
                zoomHeightPerStep,
                minimumCameraHeight,
                maximumCameraHeight);
        }

        void UpdateSmoothZoom(Camera mainCamera)
        {
            if (!zoomTargetsInitialized || mainCamera == null || worldCamRoot == null)
                return;

            float lerpFactor = CalculateZoomLerpFactor(zoomLerpSpeed, Time.unscaledDeltaTime);
            mainCamera.fieldOfView = Mathf.Lerp(
                mainCamera.fieldOfView,
                targetFieldOfView,
                lerpFactor);

            Vector3 position = worldCamRoot.position;
            position.y = Mathf.Lerp(position.y, targetCameraHeight, lerpFactor);
            worldCamRoot.position = position;

            if (Mathf.Abs(mainCamera.fieldOfView - targetFieldOfView) < 0.01f)
                mainCamera.fieldOfView = targetFieldOfView;

            if (Mathf.Abs(worldCamRoot.position.y - targetCameraHeight) < 0.01f)
            {
                position = worldCamRoot.position;
                position.y = targetCameraHeight;
                worldCamRoot.position = position;
            }
        }

        internal static float CalculateZoomFieldOfView(
            float currentFieldOfView,
            float rawScrollDelta,
            float zoomSensitivity,
            float minimumFieldOfView,
            float maximumFieldOfView)
        {
            float normalizedScrollDelta = NormalizeScrollDelta(rawScrollDelta);
            float zoomDegrees = normalizedScrollDelta * Mathf.Max(0f, zoomSensitivity) * 0.05f;
            float minimum = Mathf.Min(minimumFieldOfView, maximumFieldOfView);
            float maximum = Mathf.Max(minimumFieldOfView, maximumFieldOfView);
            return Mathf.Clamp(currentFieldOfView - zoomDegrees, minimum, maximum);
        }

        internal static float NormalizeScrollDelta(float rawScrollDelta)
        {
            if (Mathf.Abs(rawScrollDelta) < 0.001f)
                return 0f;

            float normalizedScrollDelta = Mathf.Abs(rawScrollDelta) >= 10f
                ? rawScrollDelta / 120f
                : rawScrollDelta;
            return Mathf.Clamp(normalizedScrollDelta, -3f, 3f);
        }

        internal static float CalculateZoomHeight(
            float currentHeight,
            float rawScrollDelta,
            float heightPerStep,
            float minimumHeight,
            float maximumHeight)
        {
            float normalizedScrollDelta = NormalizeScrollDelta(rawScrollDelta);
            float minimum = Mathf.Min(minimumHeight, maximumHeight);
            float maximum = Mathf.Max(minimumHeight, maximumHeight);
            float heightChange = normalizedScrollDelta * Mathf.Max(0f, heightPerStep);
            return Mathf.Clamp(currentHeight - heightChange, minimum, maximum);
        }

        internal static float CalculateZoomLerpFactor(float lerpSpeed, float deltaTime)
        {
            return 1f - Mathf.Exp(-Mathf.Max(0.01f, lerpSpeed) * Mathf.Max(0f, deltaTime));
        }

        internal static Vector3 ClampPositionToBounds(
            Vector3 position,
            Vector2 boundsMinimum,
            Vector2 boundsMaximum,
            float padding)
        {
            float minimumX = Mathf.Min(boundsMinimum.x, boundsMaximum.x) + Mathf.Max(0f, padding);
            float maximumX = Mathf.Max(boundsMinimum.x, boundsMaximum.x) - Mathf.Max(0f, padding);
            float minimumZ = Mathf.Min(boundsMinimum.y, boundsMaximum.y) + Mathf.Max(0f, padding);
            float maximumZ = Mathf.Max(boundsMinimum.y, boundsMaximum.y) - Mathf.Max(0f, padding);

            if (minimumX > maximumX)
                minimumX = maximumX = (boundsMinimum.x + boundsMaximum.x) * 0.5f;

            if (minimumZ > maximumZ)
                minimumZ = maximumZ = (boundsMinimum.y + boundsMaximum.y) * 0.5f;

            position.x = Mathf.Clamp(position.x, minimumX, maximumX);
            position.z = Mathf.Clamp(position.z, minimumZ, maximumZ);
            return position;
        }

        void OnValidate()
        {
            zoomSpeed = Mathf.Max(0f, zoomSpeed);
            zoomHeightPerStep = Mathf.Max(0f, zoomHeightPerStep);
            zoomLerpSpeed = Mathf.Max(0.01f, zoomLerpSpeed);
            minimumFieldOfView = Mathf.Clamp(minimumFieldOfView, 1f, 179f);
            maximumFieldOfView = Mathf.Clamp(maximumFieldOfView, 1f, 179f);
            mapBoundsPadding = Mathf.Max(0f, mapBoundsPadding);
        }
    }

}
