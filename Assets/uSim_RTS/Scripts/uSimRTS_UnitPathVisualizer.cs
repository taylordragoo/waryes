using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace uSimRTS
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(uSimRTS_Unit))]
    public class uSimRTS_UnitPathVisualizer : MonoBehaviour
    {
        [Tooltip("Show the unit's remaining movement route during play mode.")]
        public bool showPath = true;
        [Tooltip("Only show routes for units selected by the RTS selection system.")]
        public bool selectedUnitsOnly = true;
        [Tooltip("Color used when the unit has selected a road-assisted route.")]
        public Color roadPathColor = Color.cyan;
        [Tooltip("Color used for a calculated terrain-only NavMesh route.")]
        public Color terrainPathColor = Color.yellow;
        [Tooltip("Color used when the unit is moving directly because no calculated route is available.")]
        public Color fallbackPathColor = new Color(1f, 0.55f, 0f, 1f);
        [Min(0.01f)] public float pathWidth = 0.12f;
        [Min(0f)] public float pathHeight = 0.2f;

        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        static readonly int ColorId = Shader.PropertyToID("_Color");

        readonly List<Vector3> routePoints = new List<Vector3>(16);

        uSimRTS_Unit unit;
        LineRenderer routeRenderer;
        Material routeMaterial;

        void Awake()
        {
            unit = GetComponent<uSimRTS_Unit>();
        }

        void LateUpdate()
        {
            UpdateRouteRenderer();
        }

        void OnDestroy()
        {
            if (routeMaterial != null)
                Destroy(routeMaterial);
        }

        void CreateRouteRenderer()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            if (shader == null)
            {
                Debug.LogWarning("No supported shader was found for the unit route visualizer.", this);
                enabled = false;
                return;
            }

            GameObject routeObject = new GameObject("Route Debug Path");
            routeObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
            routeObject.transform.SetParent(transform, false);

            routeMaterial = new Material(shader)
            {
                name = "Unit Route Debug Material",
                hideFlags = HideFlags.HideAndDontSave
            };

            routeRenderer = routeObject.AddComponent<LineRenderer>();
            routeRenderer.sharedMaterial = routeMaterial;
            routeRenderer.useWorldSpace = true;
            routeRenderer.loop = false;
            routeRenderer.alignment = LineAlignment.View;
            routeRenderer.textureMode = LineTextureMode.Stretch;
            routeRenderer.numCapVertices = 4;
            routeRenderer.numCornerVertices = 4;
            routeRenderer.shadowCastingMode = ShadowCastingMode.Off;
            routeRenderer.receiveShadows = false;
            routeRenderer.lightProbeUsage = LightProbeUsage.Off;
            routeRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            routeRenderer.allowOcclusionWhenDynamic = false;
            routeRenderer.sortingOrder = 100;
            routeRenderer.enabled = false;
        }

        void UpdateRouteRenderer()
        {
            if (unit == null || !showPath)
            {
                HideRoute();
                return;
            }

            bool isSelected = unit.selector != null && unit.selector.activeSelf;
            if (selectedUnitsOnly && !isSelected)
            {
                HideRoute();
                return;
            }

            if (!unit.TryBuildDebugRoute(routePoints, out uSimRTS_RouteDebugKind routeKind))
            {
                HideRoute();
                return;
            }

            if (routeRenderer == null)
                CreateRouteRenderer();

            if (routeRenderer == null)
                return;

            Color routeColor = fallbackPathColor;

            if (routeKind == uSimRTS_RouteDebugKind.Road)
                routeColor = roadPathColor;
            else if (routeKind == uSimRTS_RouteDebugKind.Terrain)
                routeColor = terrainPathColor;

            SetRouteColor(routeColor);

            float scaledWidth = unit.size * 0.15f;
            routeRenderer.widthMultiplier = Mathf.Max(0.01f, Mathf.Max(pathWidth, scaledWidth));
            routeRenderer.positionCount = routePoints.Count;

            float scaledHeight = unit.size * 0.2f;
            Vector3 heightOffset = Vector3.up * Mathf.Max(pathHeight, scaledHeight);
            for (int i = 0; i < routePoints.Count; i++)
                routeRenderer.SetPosition(i, routePoints[i] + heightOffset);

            routeRenderer.enabled = true;
        }

        void SetRouteColor(Color color)
        {
            routeRenderer.startColor = color;
            routeRenderer.endColor = color;
            routeMaterial.color = color;

            if (routeMaterial.HasProperty(BaseColorId))
                routeMaterial.SetColor(BaseColorId, color);

            if (routeMaterial.HasProperty(ColorId))
                routeMaterial.SetColor(ColorId, color);
        }

        void HideRoute()
        {
            if (routeRenderer == null)
                return;

            routeRenderer.enabled = false;
            routeRenderer.positionCount = 0;
        }
    }
}
