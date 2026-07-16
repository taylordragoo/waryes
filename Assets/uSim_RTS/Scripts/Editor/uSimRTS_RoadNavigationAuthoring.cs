using System;
using System.Collections.Generic;
using System.IO;
using JBooth.MicroVerseCore;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace uSimRTS.Editor
{
    public static class uSimRTS_RoadNavigationAuthoring
    {
        const string RoadAreaName = "Road";
        const string RoadCenterAreaName = "RoadCenter";
        const string NavMeshAreasPath = "ProjectSettings/NavMeshAreas.asset";
        const string PrepareMenuPath = "Tools/uSim RTS/Navigation/Prepare MicroVerse Roads";
        const string PrepareAndBakeMenuPath = "Tools/uSim RTS/Navigation/Prepare MicroVerse Roads and Bake";
        const string CenterVolumeName = "__uSimRTS Road Center Bake Volume";
        const float CenterWidthRatio = 0.35f;
        const float MinimumCenterWidth = 1.5f;
        const float CenterVolumeHeight = 20f;
        const float CenterVolumeOverlapRatio = 0.5f;

        static List<BakeOperation> pendingBakeOperations;
        static List<GameObject> pendingCenterBakeVolumes;
        static List<string> pendingScenePaths;
        static PreparationResult pendingPreparationResult;

        [MenuItem(PrepareMenuPath)]
        public static void PrepareOpenScenes()
        {
            if (!TryPrepareOpenScenes(out PreparationResult result, false))
                return;

            Debug.Log(
                $"Prepared MicroVerse road navigation: {result.roadSystems} RoadSystem roots, " +
                $"{result.roads} roads, {result.intersections} intersections, " +
                $"{result.addedModifiers} new NavMeshModifier components. " +
                "Road-center ribbons are generated transiently by the Prepare and Bake command.");
        }

        [MenuItem(PrepareAndBakeMenuPath)]
        public static void PrepareOpenScenesAndBake()
        {
            if (pendingBakeOperations != null)
            {
                Debug.LogWarning("A road navigation bake is already running.");
                return;
            }

            CleanupCenterBakeVolumes();

            if (!TryPrepareOpenScenes(out PreparationResult result, true))
            {
                CleanupCenterBakeVolumes();
                return;
            }

            Unity.AI.Navigation.NavMeshSurface[] surfaces =
                Object.FindObjectsByType<Unity.AI.Navigation.NavMeshSurface>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);

            List<BakeOperation> bakeOperations = new List<BakeOperation>();
            HashSet<NavMeshData> assignedData = new HashSet<NavMeshData>();
            foreach (Unity.AI.Navigation.NavMeshSurface surface in surfaces)
            {
                if (EditorUtility.IsPersistent(surface) || !surface.gameObject.scene.IsValid() || !surface.isActiveAndEnabled)
                    continue;

                NavMeshData data = GetOrCreatePersistentNavMeshData(surface, assignedData);
                if (data == null)
                    continue;

                AsyncOperation operation = surface.UpdateNavMesh(data);
                if (operation != null)
                    bakeOperations.Add(new BakeOperation(surface, data, operation));
            }

            if (bakeOperations.Count == 0)
            {
                CleanupCenterBakeVolumes();
                Debug.LogWarning(
                    "MicroVerse roads were prepared, but no active Unity AI Navigation NavMeshSurface was found in the open scenes. " +
                    "Add or enable a NavMeshSurface, then run the command again.");
                return;
            }

            pendingBakeOperations = bakeOperations;
            pendingScenePaths = GetOpenScenePaths();
            pendingPreparationResult = result;
            EditorApplication.update -= CompleteBakeWhenReady;
            EditorApplication.update += CompleteBakeWhenReady;

            Debug.Log(
                $"Prepared MicroVerse roads and started {bakeOperations.Count} persistent NavMeshSurface bake(s).");
        }

        static void CompleteBakeWhenReady()
        {
            if (pendingBakeOperations == null)
            {
                EditorApplication.update -= CompleteBakeWhenReady;
                return;
            }

            foreach (BakeOperation bake in pendingBakeOperations)
            {
                if (bake.operation != null && !bake.operation.isDone)
                    return;
            }

            EditorApplication.update -= CompleteBakeWhenReady;

            foreach (BakeOperation bake in pendingBakeOperations)
                EditorUtility.SetDirty(bake.data);

            int bakedSurfaces = pendingBakeOperations.Count;
            PreparationResult result = pendingPreparationResult;
            List<string> scenePaths = pendingScenePaths;
            int roadTriangles = CountNavMeshTriangles(NavMesh.GetAreaFromName(RoadAreaName));
            int roadCenterTriangles = CountNavMeshTriangles(NavMesh.GetAreaFromName(RoadCenterAreaName));

            CleanupCenterBakeVolumes();

            AssetDatabase.SaveAssets();
            bool scenesSaved = EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();

            if (scenesSaved && scenePaths.Count > 0)
            {
                AssetDatabase.ForceReserializeAssets(
                    scenePaths,
                    ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata);
            }

            pendingBakeOperations = null;
            pendingScenePaths = null;

            Debug.Log(
                $"Prepared and baked road navigation: {result.roadSystems} RoadSystem roots, " +
                $"{result.roads} roads, {result.intersections} intersections, " +
                $"{result.addedModifiers} new modifiers, {result.centerVolumes} transient center volumes, " +
                $"{bakedSurfaces} persistent NavMeshSurface bake(s). " +
                $"Road triangles: {roadTriangles}; RoadCenter triangles: {roadCenterTriangles}. " +
                $"Open scenes saved: {scenesSaved}.");

            if (roadTriangles == 0)
            {
                Debug.LogWarning(
                    "The bake completed but produced no Road-area triangles. Verify that the NavMeshSurface includes the road layers and geometry.");
            }

            if (roadCenterTriangles == 0)
            {
                Debug.LogWarning(
                    "The bake completed but produced no RoadCenter-area triangles. Verify the MicroVerse roads have valid splines and widths.");
            }
        }

        static bool TryPrepareOpenScenes(out PreparationResult result, bool createCenterVolumes)
        {
            result = new PreparationResult();

            if (!TryEnsureNavigationAreas(out int roadArea, out int roadCenterArea))
                return false;

            if (createCenterVolumes)
                pendingCenterBakeVolumes = new List<GameObject>();

            RoadSystem[] roadSystems = Object.FindObjectsByType<RoadSystem>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            if (roadSystems.Length == 0)
            {
                Debug.LogWarning("No MicroVerse RoadSystem components were found in the open scenes.");
                return false;
            }

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Prepare MicroVerse Road Navigation");

            HashSet<GameObject> preparedObjects = new HashSet<GameObject>();

            foreach (RoadSystem roadSystem in roadSystems)
            {
                if (EditorUtility.IsPersistent(roadSystem) || !roadSystem.gameObject.scene.IsValid())
                    continue;

                result.roadSystems++;

                Road[] roads = roadSystem.GetComponentsInChildren<Road>(true);
                result.roads += roads.Length;
                foreach (Road road in roads)
                {
                    PrepareObject(road.gameObject, roadArea, preparedObjects, ref result);
                    if (createCenterVolumes)
                        result.centerVolumes += CreateRoadCenterVolumes(road, roadCenterArea);
                }

                Intersection[] intersections = roadSystem.GetComponentsInChildren<Intersection>(true);
                result.intersections += intersections.Length;
                foreach (Intersection intersection in intersections)
                    PrepareObject(intersection.gameObject, roadArea, preparedObjects, ref result);
            }

            Undo.CollapseUndoOperations(undoGroup);
            AssetDatabase.SaveAssets();
            return result.roadSystems > 0;
        }

        static void PrepareObject(
            GameObject target,
            int roadArea,
            HashSet<GameObject> preparedObjects,
            ref PreparationResult result)
        {
            if (!preparedObjects.Add(target))
                return;

            Unity.AI.Navigation.NavMeshModifier modifier =
                target.GetComponent<Unity.AI.Navigation.NavMeshModifier>();

            if (modifier == null)
            {
                modifier = Undo.AddComponent<Unity.AI.Navigation.NavMeshModifier>(target);
                result.addedModifiers++;
            }
            else
            {
                Undo.RecordObject(modifier, "Configure Road NavMesh Area");
            }

            modifier.overrideArea = true;
            modifier.area = roadArea;
            modifier.applyToChildren = true;

            EditorUtility.SetDirty(modifier);
            EditorSceneManager.MarkSceneDirty(target.scene);
        }

        static int CreateRoadCenterVolumes(Road road, int roadCenterArea)
        {
            if (road == null ||
                road.splineContainer == null ||
                road.splineContainer.Splines == null ||
                road.splineContainer.Splines.Count == 0)
            {
                return 0;
            }

            float roadWidth = road.config != null
                ? road.config.modelWidth
                : 8f;
            Vector3 roadScale = road.transform.lossyScale;
            roadWidth *= Mathf.Max(Mathf.Abs(roadScale.x), Mathf.Abs(roadScale.z));

            float centerWidth = Mathf.Max(MinimumCenterWidth, roadWidth * CenterWidthRatio);
            float segmentSpacing = Mathf.Clamp(roadWidth * 0.75f, 4f, 12f);
            int createdVolumes = 0;

            for (int splineIndex = 0; splineIndex < road.splineContainer.Splines.Count; splineIndex++)
            {
                float splineLength = road.splineContainer.CalculateLength(splineIndex);
                if (splineLength <= 0.01f)
                    continue;

                int segmentCount = Mathf.Max(1, Mathf.CeilToInt(splineLength / segmentSpacing));
                Vector3 previousPosition = road.splineContainer.EvaluatePosition(splineIndex, 0f);

                for (int segmentIndex = 1; segmentIndex <= segmentCount; segmentIndex++)
                {
                    float normalizedDistance = segmentIndex / (float)segmentCount;
                    Vector3 currentPosition = road.splineContainer.EvaluatePosition(
                        splineIndex,
                        normalizedDistance);
                    Vector3 planarDirection = currentPosition - previousPosition;
                    planarDirection.y = 0f;

                    if (planarDirection.sqrMagnitude <= 0.0001f)
                    {
                        previousPosition = currentPosition;
                        continue;
                    }

                    GameObject volumeObject = new GameObject(CenterVolumeName)
                    {
                        hideFlags = HideFlags.HideAndDontSave,
                        layer = 0
                    };

                    SceneManager.MoveGameObjectToScene(volumeObject, road.gameObject.scene);
                    volumeObject.transform.SetPositionAndRotation(
                        (previousPosition + currentPosition) * 0.5f,
                        Quaternion.LookRotation(planarDirection.normalized, Vector3.up));

                    Unity.AI.Navigation.NavMeshModifierVolume volume =
                        volumeObject.AddComponent<Unity.AI.Navigation.NavMeshModifierVolume>();
                    volume.area = roadCenterArea;
                    volume.center = Vector3.zero;
                    volume.size = new Vector3(
                        centerWidth,
                        CenterVolumeHeight,
                        planarDirection.magnitude + (centerWidth * CenterVolumeOverlapRatio));

                    pendingCenterBakeVolumes.Add(volumeObject);
                    createdVolumes++;
                    previousPosition = currentPosition;
                }
            }

            return createdVolumes;
        }

        static void CleanupCenterBakeVolumes()
        {
            if (pendingCenterBakeVolumes == null)
                return;

            foreach (GameObject volumeObject in pendingCenterBakeVolumes)
            {
                if (volumeObject != null)
                    Object.DestroyImmediate(volumeObject);
            }

            pendingCenterBakeVolumes = null;
        }

        static bool TryEnsureNavigationAreas(out int roadArea, out int roadCenterArea)
        {
            if (!TryEnsureNavMeshArea(RoadAreaName, 1f, out roadArea))
            {
                roadCenterArea = -1;
                return false;
            }

            return TryEnsureNavMeshArea(RoadCenterAreaName, 1f, out roadCenterArea);
        }

        static bool TryEnsureNavMeshArea(string areaName, float cost, out int areaIndex)
        {
            areaIndex = NavMesh.GetAreaFromName(areaName);
            if (areaIndex >= 0)
                return true;

            Object[] settingsAssets = AssetDatabase.LoadAllAssetsAtPath(NavMeshAreasPath);
            if (settingsAssets.Length == 0)
            {
                Debug.LogError($"Could not load {NavMeshAreasPath}; the {areaName} NavMesh area was not created.");
                return false;
            }

            SerializedObject settings = new SerializedObject(settingsAssets[0]);
            SerializedProperty areas = settings.FindProperty("areas");
            if (areas == null || !areas.isArray)
            {
                Debug.LogError($"Could not find the NavMesh area list in {NavMeshAreasPath}.");
                return false;
            }

            int emptyArea = -1;
            for (int i = 0; i < areas.arraySize; i++)
            {
                SerializedProperty area = areas.GetArrayElementAtIndex(i);
                SerializedProperty name = area.FindPropertyRelative("name");

                if (name.stringValue == areaName)
                {
                    areaIndex = i;
                    return true;
                }

                if (i >= 3 && emptyArea < 0 && string.IsNullOrEmpty(name.stringValue))
                    emptyArea = i;
            }

            if (emptyArea < 0)
            {
                Debug.LogError($"No empty NavMesh area slot is available for the {areaName} area.");
                return false;
            }

            SerializedProperty areaProperty = areas.GetArrayElementAtIndex(emptyArea);
            areaProperty.FindPropertyRelative("name").stringValue = areaName;
            areaProperty.FindPropertyRelative("cost").floatValue = cost;
            settings.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(settingsAssets[0]);
            AssetDatabase.SaveAssets();

            areaIndex = emptyArea;
            return true;
        }

        static NavMeshData GetOrCreatePersistentNavMeshData(
            Unity.AI.Navigation.NavMeshSurface surface,
            HashSet<NavMeshData> assignedData)
        {
            NavMeshData data = surface.navMeshData;
            if (data != null &&
                !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(data)) &&
                assignedData.Add(data))
            {
                return data;
            }

            data = FindExistingNavMeshData(surface, assignedData);
            if (data == null)
                data = CreateNavMeshDataAsset(surface);

            if (data == null)
            {
                Debug.LogError($"Could not create persistent NavMeshData for {surface.name}.", surface);
                return null;
            }

            SetNavMeshData(surface, data);
            assignedData.Add(data);
            return data;
        }

        static NavMeshData FindExistingNavMeshData(
            Unity.AI.Navigation.NavMeshSurface surface,
            HashSet<NavMeshData> assignedData)
        {
            string scenePath = surface.gameObject.scene.path;
            string sceneDirectory = Path.GetDirectoryName(scenePath)?.Replace('\\', '/');
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            if (string.IsNullOrEmpty(sceneDirectory) || string.IsNullOrEmpty(sceneName))
                return null;

            string[] assetGuids = AssetDatabase.FindAssets("t:NavMeshData", new[] { sceneDirectory });
            foreach (string guid in assetGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string assetName = Path.GetFileNameWithoutExtension(assetPath);
                NavMeshData data = AssetDatabase.LoadAssetAtPath<NavMeshData>(assetPath);

                if (data != null &&
                    !assignedData.Contains(data) &&
                    assetName.IndexOf(sceneName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return data;
                }
            }

            return null;
        }

        static NavMeshData CreateNavMeshDataAsset(Unity.AI.Navigation.NavMeshSurface surface)
        {
            string scenePath = surface.gameObject.scene.path;
            string sceneDirectory = Path.GetDirectoryName(scenePath)?.Replace('\\', '/');
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            if (string.IsNullOrEmpty(sceneDirectory) || string.IsNullOrEmpty(sceneName))
                return null;

            string dataDirectory = $"{sceneDirectory}/{sceneName}";
            if (!AssetDatabase.IsValidFolder(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
                AssetDatabase.Refresh();
            }

            NavMeshData data = new NavMeshData(surface.agentTypeID)
            {
                name = surface.name
            };

            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{dataDirectory}/NavMesh-{surface.name}.asset");
            AssetDatabase.CreateAsset(data, assetPath);
            return data;
        }

        static void SetNavMeshData(Unity.AI.Navigation.NavMeshSurface surface, NavMeshData data)
        {
            NavMeshData previousData = surface.navMeshData;
            surface.RemoveData();

            SerializedObject serializedSurface = new SerializedObject(surface);
            SerializedProperty navMeshData = serializedSurface.FindProperty("m_NavMeshData");
            navMeshData.objectReferenceValue = data;
            serializedSurface.ApplyModifiedPropertiesWithoutUndo();

            if (previousData != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(previousData)))
                Object.DestroyImmediate(previousData);

            if (surface.isActiveAndEnabled)
                surface.AddData();

            EditorUtility.SetDirty(surface);
            EditorSceneManager.MarkSceneDirty(surface.gameObject.scene);
        }

        static List<string> GetOpenScenePaths()
        {
            List<string> paths = new List<string>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && !string.IsNullOrEmpty(scene.path))
                    paths.Add(scene.path);
            }

            return paths;
        }

        static int CountNavMeshTriangles(int area)
        {
            if (area < 0)
                return 0;

            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            int triangleCount = 0;

            foreach (int triangleArea in triangulation.areas)
            {
                if (triangleArea == area)
                    triangleCount++;
            }

            return triangleCount;
        }

        struct PreparationResult
        {
            public int roadSystems;
            public int roads;
            public int intersections;
            public int addedModifiers;
            public int centerVolumes;
        }

        readonly struct BakeOperation
        {
            public readonly Unity.AI.Navigation.NavMeshSurface surface;
            public readonly NavMeshData data;
            public readonly AsyncOperation operation;

            public BakeOperation(
                Unity.AI.Navigation.NavMeshSurface surface,
                NavMeshData data,
                AsyncOperation operation)
            {
                this.surface = surface;
                this.data = data;
                this.operation = operation;
            }
        }
    }
}
