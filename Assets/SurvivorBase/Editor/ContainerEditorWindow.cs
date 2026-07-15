using System.Collections.Generic;
using System.Linq;
using SurvivorBase.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SurvivorBase.Editor
{
    public class ContainerEditorWindow : EditorWindow
    {
        [SerializeField] private ContainerData containerData;

        private GameObject _selectedContainer;

        private ContainerEntry _selectedContainerEntry;
        private SideType _selectedSide;
        private SideModuleData _selectedModuleData;
        private WallDecalHandler _wallHandler;

        private List<PillarIdentifier> _pillarIdentifiers;
        private ContainerMaterialSet[] _materialSets;

        private Vector2 _scrollPosition;

        private int _selectedMaterialIndex;
        private float _wornLevel = 1;

        private Texture _pressedTexture;
        private Texture2D[] _decalIcons;

        private Dictionary<SideType, GameObject> _sideStates = new Dictionary<SideType, GameObject>();
        private Dictionary<SideType, SideType> _sideTypesPair;

        private Dictionary<Material, Dictionary<string, float>> _originalMaterialValues =
            new Dictionary<Material, Dictionary<string, float>>();

        private GameObject _selectedDecal;
        private List<GameObject> _selectedDecals;
        private bool _hasWallHandler;
        private bool _enableWornLevel = false;


        private Dictionary<GameObject, bool> _containerWornLevelState = new Dictionary<GameObject, bool>();
        private Dictionary<GameObject, Material[]> _originalMaterials = new Dictionary<GameObject, Material[]>();
        private Dictionary<GameObject, Material[]> _originalPropsMaterials = new Dictionary<GameObject, Material[]>();
        private Dictionary<GameObject, Material[]> _originalDecalMaterials = new Dictionary<GameObject, Material[]>();


        [MenuItem("Tools/Container Editor")]
        public static void ShowWindow()
        {
            GetWindow<ContainerEditorWindow>("Container Editor");
        }


        private void OnEnable()
        {
            if (containerData == null)
            {
                containerData =
                    AssetDatabase.LoadAssetAtPath<ContainerData>("Assets/SurvivorBase/Editor/Data/ContainerData.asset");
            }

            _pressedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SurvivorBase/Editor/Icons/Pressed.png");
            _materialSets = Resources.LoadAll<ContainerMaterialSet>("Materials/");
            _decalIcons = new Texture2D[]
            {
                AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SurvivorBase/Editor/Icons/Decal1.png"),
                AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SurvivorBase/Editor/Icons/Decal2.png"),
                AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SurvivorBase/Editor/Icons/Decal3.png"),
                AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SurvivorBase/Editor/Icons/Decal4.png")
            };
        }

        private void OnSelectionChange()
        {
            _sideTypesPair = new Dictionary<SideType, SideType>
            {
                { SideType.Side_L1, SideType.Side_L2 },
                { SideType.Side_R2, SideType.Side_R1 },
                { SideType.Top2, SideType.Top1 }
            };

            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject != null)
            {

                if (selectedObject.GetComponent<ContainerTypeIdentifier>() != null)
                    _pillarIdentifiers = selectedObject.GetComponentsInChildren<PillarIdentifier>().ToList();
                if (selectedObject.CompareTag("Container"))
                {
                    HandleContainerSelection(selectedObject);
                    if (!_containerWornLevelState.ContainsKey(selectedObject))
                    {
                        _containerWornLevelState[selectedObject] = false;
                    }

                    _enableWornLevel = _containerWornLevelState[selectedObject];
                    if (_enableWornLevel)
                    {
                        CreateMaterialInstances();
                    }
                    else
                    {
                        RestoreOriginalMaterials();
                    }
                }
                else
                {
                    var sideIdentifier = selectedObject.GetComponentInParent<SideIdentifier>();
                    if (sideIdentifier != null)
                    {
                        HandleSideSelection(sideIdentifier);
                    }
                    else
                    {
                        ResetSelection();
                    }
                }
            }
            else
            {
                ResetSelection();
            }

            _hasWallHandler = false;

            if (selectedObject != null)
            {
                _wallHandler = selectedObject.GetComponentInChildren<WallDecalHandler>()
                               ?? selectedObject.transform.parent?.GetComponentInChildren<WallDecalHandler>();

                _selectedDecals = _wallHandler != null ? _wallHandler.GetActiveDecals() : new List<GameObject>();
                _hasWallHandler = _wallHandler != null;
                if (_wallHandler == null)
                {
                    _selectedDecals = new List<GameObject>();
                }


                if (_wallHandler != null)
                {
                    _selectedDecals = _wallHandler.GetActiveDecals();
                    _hasWallHandler = true;
                }
                else
                {
                    _selectedDecals.Clear();
                }
            }

            Repaint();
        }
        private void StoreOriginalDecalMaterials()
        {
            if (_wallHandler == null) return;

            foreach (var decal in _wallHandler.GetActiveDecals())
            {
                if (!_originalDecalMaterials.ContainsKey(decal))
                {
                    var renderers = decal.GetComponentsInChildren<Renderer>(true);
                    List<Material> mats = new List<Material>();
                    foreach (var r in renderers)
                        mats.AddRange(r.sharedMaterials);
                    _originalDecalMaterials[decal] = mats.ToArray();
                }
            }
        }
        private void RestoreOriginalDecalMaterials()
        {
            if (_wallHandler == null) return;

            foreach (var decal in _wallHandler.GetActiveDecals())
            {
                if (_originalDecalMaterials.TryGetValue(decal, out var originalMats))
                {
                    var renderers = decal.GetComponentsInChildren<Renderer>(true);
                    int index = 0;
                    foreach (var r in renderers)
                    {
                        int count = r.sharedMaterials.Length;
                        r.sharedMaterials = originalMats.Skip(index).Take(count).ToArray();
                        index += count;
                    }
                }
            }
            _originalDecalMaterials.Clear();
        }



        
        private void DrawWearLevelControls()
        {
            GUILayout.BeginHorizontal();
            bool previousEnableWornLevel = _enableWornLevel;
            _enableWornLevel = EditorGUILayout.Toggle("Worn Effect-For this Level", _enableWornLevel);

            if (_selectedContainer != null)
            {
                _containerWornLevelState[_selectedContainer] = _enableWornLevel;
            }

            if (_enableWornLevel != previousEnableWornLevel)
            {
                if (_wallHandler != null)
                {
                    _wallHandler.RefreshDecalMaterials(_materialSets[_selectedMaterialIndex].FrameMaterial, _wornLevel, _enableWornLevel);
                }
                if (_enableWornLevel)
                {
                    CreateMaterialInstances();
                }
                else
                {
                    ApplyMaterialSetAsSharedReferences();
                }
            }

            GUILayout.EndHorizontal();

            if (_enableWornLevel)
            {
                DrawWearLevelSlider();
            }
        }
        private void ApplyMaterialSetAsSharedReferences()
        {
            if (_selectedContainer == null || _materialSets == null || _selectedMaterialIndex >= _materialSets.Length)
                return;

            var materialSet = _materialSets[_selectedMaterialIndex];

            var renderers = _selectedContainer.GetComponentsInChildren<Renderer>(true);

            foreach (var renderer in renderers)
            {
                Material[] sharedMaterials = renderer.sharedMaterials;

                for (int i = 0; i < sharedMaterials.Length; i++)
                {
                    var mat = sharedMaterials[i];
                    if (mat == null) continue;

                    string matName = mat.name;

                    if (matName.StartsWith("MI_Container_"))
                        sharedMaterials[i] = materialSet.FrameMaterial;
                    else if (matName.StartsWith("Details_Conteiners_"))
                        sharedMaterials[i] = materialSet.DetailsMaterial;
                    else if (matName.StartsWith("MI_Valves_Paint"))
                        sharedMaterials[i] = materialSet.ValvesMaterial;
                    else if (matName.StartsWith("MI_WoodFloor_B"))
                        sharedMaterials[i] = materialSet.FloorMaterial;
                    else if (matName.StartsWith("MI_Conteiners_Paint-A_Grey"))
                        sharedMaterials[i] = materialSet.WallInsideMaterial;
                    else if (matName.StartsWith("MI_Floor_Paint"))
                        sharedMaterials[i] = materialSet.StairsMaterial;
                    else if (matName.StartsWith("MI_Blend_Conteiners_Metal-A"))
                        sharedMaterials[i] = materialSet.DoorMaterial;
                }

                renderer.sharedMaterials = sharedMaterials;
            }
        }

        private void CreateMaterialInstances()
        {
            if (_selectedContainer == null) return;

            var renderers = _selectedContainer.GetComponentsInChildren<Renderer>(true);

            if (!_originalMaterials.ContainsKey(_selectedContainer))
            {
                _originalMaterials[_selectedContainer] = renderers.SelectMany(r => r.sharedMaterials).ToArray();
            }

            foreach (var renderer in renderers)
            {
                var sharedMats = renderer.sharedMaterials;
                var newMats = new Material[sharedMats.Length];

                for (int i = 0; i < sharedMats.Length; i++)
                {
                    var mat = sharedMats[i];
                    if (mat == null)
                    {
                        newMats[i] = null;
                        continue;
                    }

                    string matName = mat.name;

                    bool shouldInstance =
                        matName.StartsWith("MI_Container_") ||
                        matName.StartsWith("Details_Conteiners_") ||
                        matName.StartsWith("MI_Valves_Paint") ||
                        matName.StartsWith("MI_WoodFloor_B") ||
                        matName.StartsWith("MI_Floor_Paint") ||
                        matName.StartsWith("MI_Conteiners_Paint-A_Grey") ||
                        matName.StartsWith("MI_Blend_Conteiners_Metal-A");

                    if (shouldInstance)
                    {
                        var newMat = new Material(mat);
                        newMat.SetFloat("_Worn_Level", _wornLevel);
                        newMats[i] = newMat;
                    }
                    else
                    {
                        newMats[i] = mat;
                    }
                }

                renderer.sharedMaterials = newMats;
            }
        }



        private void RestoreOriginalMaterials()
        {
            if (_selectedContainer == null || !_originalMaterials.ContainsKey(_selectedContainer)) return;

            var renderers = _selectedContainer.GetComponentsInChildren<Renderer>(true);
            int index = 0;
            foreach (var renderer in renderers)
            {
                Material[] originalMats = _originalMaterials[_selectedContainer].Skip(index).Take(renderer.sharedMaterials.Length).ToArray();
                Material[] restoredMats = new Material[originalMats.Length];

                for (int i = 0; i < originalMats.Length; i++)
                {
                    string matName = originalMats[i]?.name;
                    if (string.IsNullOrEmpty(matName)) continue;

                    bool wasInstance =
                        matName.StartsWith("MI_Container_") ||
                        matName.StartsWith("Details_Conteiners_") ||
                        matName.StartsWith("MI_Valves_Paint") ||
                        matName.StartsWith("MI_WoodFloor_B") ||
                        matName.StartsWith("MI_Floor_Paint") ||
                        matName.StartsWith("MI_Conteiners_Paint-A_Grey") ||
                        matName.StartsWith("MI_Blend_Conteiners_Metal-A");

                    restoredMats[i] = wasInstance ? GetMaterialFromSet(matName) : renderer.sharedMaterials[i];
                }

                renderer.sharedMaterials = restoredMats;
                index += renderer.sharedMaterials.Length;
            }

            _originalMaterials.Remove(_selectedContainer);
            Repaint();
        }


        private void DrawMaterialSelection()
        {
            GUILayout.Label("Material Selection:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            for (int i = 0; i < _materialSets.Length; i++)
            {
                bool isSelected = _selectedMaterialIndex == i;
                GUI.backgroundColor = isSelected ? Color.gray : Color.white;

                if (GUILayout.Button(new GUIContent(_materialSets[i].Icon, "Material Set " + (i + 1)),
                        GUILayout.Width(55), GUILayout.Height(55)))
                {
                    _selectedMaterialIndex = i;
                    ApplyMaterialToSelectedContainer();
                }
            }

            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
        }

        private void ApplyMaterialToSelectedContainer()
        {
            if (_selectedContainer == null) return;

            var renderers = _selectedContainer.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                Material[] sharedMaterials = renderer.sharedMaterials;

                for (int i = 0; i < sharedMaterials.Length; i++)
                {
                    Material originalMaterial = sharedMaterials[i];
                    if (originalMaterial == null) continue;

                    string matName = originalMaterial.name;

                    Material newMaterial = GetMaterialFromSet(matName);
                    if (newMaterial == null) continue;

                    newMaterial.SetFloat("_Worn_Level", _wornLevel);
                    sharedMaterials[i] = newMaterial;

                    EditorUtility.SetDirty(newMaterial);
                }

                renderer.sharedMaterials = sharedMaterials;
                EditorUtility.SetDirty(renderer);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AutoSaveContainer();
            Repaint();
        }

        private Material GetMaterialFromSet(string matName)
        {
            if (_materialSets == null || _selectedMaterialIndex >= _materialSets.Length)
                return null;

            var materialSet = _materialSets[_selectedMaterialIndex];

            if (matName.StartsWith("MI_Container_")) return materialSet.FrameMaterial;
            if (matName.StartsWith("Details_Conteiners_")) return materialSet.DetailsMaterial;
            if (matName.StartsWith("MI_Valves_Paint")) return materialSet.ValvesMaterial;
            if (matName.StartsWith("MI_WoodFloor_B")) return materialSet.FloorMaterial;
            if (matName.StartsWith("MI_Floor_Paint")) return materialSet.StairsMaterial;
            if (matName.StartsWith("MI_Conteiners_Paint-A_Grey")) return materialSet.WallInsideMaterial;
            if (matName.StartsWith("MI_Blend_Conteiners_Metal-A")) return materialSet.DoorMaterial;

            return null;
        }



        private void DrawWearLevelSlider()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Worn Level:", GUILayout.Width(80));

            EditorGUI.BeginChangeCheck();
            float newWearLevel =
                GUILayout.HorizontalSlider(_wornLevel, 0, 2, GUILayout.Width(156));
            GUILayout.Label(newWearLevel.ToString("F2"), GUILayout.Width(40));

            if (EditorGUI.EndChangeCheck())
            {
                _wornLevel = newWearLevel;
                ApplyWearLevelToContainer();
            }

            GUILayout.EndHorizontal();
        }


        private void ApplyWearLevelToContainer()
        {
            if (_selectedContainer == null)
            {
                Debug.LogWarning("No container selected.");
                return;
            }

            if (_materialSets == null || _selectedMaterialIndex >= _materialSets.Length)
            {
                Debug.LogWarning("Material set not found.");
                return;
            }

            ContainerMaterialSet selectedSet = _materialSets[_selectedMaterialIndex];

            float wornLevelNormalized = Mathf.Clamp01(_wornLevel);
            float extendedWearLevel = Mathf.Clamp(_wornLevel - 1, 0, 1);

            var allRenderers = _selectedContainer.GetComponentsInChildren<Renderer>();

            foreach (var renderer in allRenderers)
            {
                if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)
                    continue;

                Material[] newMaterials = renderer.sharedMaterials.ToArray();

                for (int i = 0; i < newMaterials.Length; i++)
                {
                    Material originalMaterial = newMaterials[i];
                    string matName = originalMaterial.name;

                    if (matName.StartsWith("MI_Container_"))
                    {
                        newMaterials[i] = new Material(selectedSet.FrameMaterial);
                    }
                    else if (matName.StartsWith("Details_Conteiners_"))
                    {
                        newMaterials[i] = new Material(selectedSet.DetailsMaterial);
                    }
                    else if (matName.StartsWith("MI_Valves_Paint"))
                    {
                        newMaterials[i] = new Material(selectedSet.ValvesMaterial);
                    }
                    else if (matName.StartsWith("MI_WoodFloor_B"))
                    {
                        newMaterials[i] = new Material(_materialSets[_selectedMaterialIndex].FloorMaterial);
                    }
                    else if (matName.StartsWith("MI_Conteiners_Paint-A_Grey"))
                    {
                        newMaterials[i] = new Material(_materialSets[_selectedMaterialIndex].WallInsideMaterial);
                    }
                    else if (matName.StartsWith("MI_Floor_Paint"))
                    {
                        newMaterials[i] = new Material(_materialSets[_selectedMaterialIndex].StairsMaterial);
                    }
                    else if (matName.StartsWith("MI_Blend_Conteiners_Metal-A"))
                    {
                        newMaterials[i] = new Material(_materialSets[_selectedMaterialIndex].DoorMaterial);
                    }
                    else
                    {
                        continue;
                    }

                    Material newMaterial = newMaterials[i];

                    if (!_originalMaterialValues.ContainsKey(originalMaterial))
                    {
                        _originalMaterialValues[originalMaterial] = new Dictionary<string, float>();
                        SaveOriginalMaterialValues(originalMaterial);
                    }

                    newMaterial.SetFloat("_Worn_Level", wornLevelNormalized);

                    if (_wornLevel > 1)
                    {
                        ApplyTargetValue(newMaterial, "_Mat2_BaseColor", 0.68f, extendedWearLevel);
                        ApplyTargetValue(newMaterial, "_Global_G", 4.11f, extendedWearLevel);
                        ApplyTargetValue(newMaterial, "_Brightness_A", 0.7f, extendedWearLevel);
                        ApplyTargetValue(newMaterial, "_Brightness_B", 0.54f, extendedWearLevel);
                    }
                }

                renderer.sharedMaterials = newMaterials;
            }

            AutoSaveContainer();
            Repaint();
        }

        private void ApplyTargetValue(Material material, string propertyName, float targetValue, float wearLevel)
        {
            if (material.HasProperty(propertyName))
            {
                if (!_originalMaterialValues.ContainsKey(material) ||
                    !_originalMaterialValues[material].ContainsKey(propertyName))
                {
                    SaveOriginalMaterialValues(material);
                }

                float originalValue = _originalMaterialValues[material][propertyName];

                material.SetFloat(propertyName, Mathf.Lerp(originalValue, targetValue, wearLevel));
            }
        }


        private void SaveOriginalMaterialValues(Material material)
        {
            if (!_originalMaterialValues.ContainsKey(material))
            {
                _originalMaterialValues[material] = new Dictionary<string, float>();
            }

            if (material.HasProperty("_Mat2_BaseColor") &&
                !_originalMaterialValues[material].ContainsKey("_Mat2_BaseColor"))
                _originalMaterialValues[material]["_Mat2_BaseColor"] = material.GetFloat("_Mat2_BaseColor");

            if (material.HasProperty("_Global_G") && !_originalMaterialValues[material].ContainsKey("_Global_G"))
                _originalMaterialValues[material]["_Global_G"] = material.GetFloat("_Global_G");

            if (material.HasProperty("_Brightness_A") &&
                !_originalMaterialValues[material].ContainsKey("_Brightness_A"))
                _originalMaterialValues[material]["_Brightness_A"] = material.GetFloat("_Brightness_A");

            if (material.HasProperty("_Brightness_B") &&
                !_originalMaterialValues[material].ContainsKey("_Brightness_B"))
                _originalMaterialValues[material]["_Brightness_B"] = material.GetFloat("_Brightness_B");
        }



        private void HandleContainerSelection(GameObject container)
        {
            _selectedContainer = container;
            _selectedContainerEntry = FindContainerEntryByType(container);

            if (_selectedContainerEntry == null)
            {
                Debug.LogWarning("ContainerEntry not found for selected container.");
                _selectedContainer = null;
                return;
            }
            _sideStates = FindAllStatesOnContainer(_selectedContainer);
            _selectedModuleData = _selectedContainerEntry.selectSideData.moduleData;
        }

        private void HandleSideSelection(SideIdentifier sideIdentifier)
        {
            _selectedSide = sideIdentifier.sideType;

            var container = sideIdentifier.GetComponentInParent<ContainerTypeIdentifier>()?.gameObject;
            if (container == null)
            {
                Debug.LogWarning("Cannot find ContainerTypeIdentifier in parent hierarchy.");
                return;
            }

            HandleContainerSelection(container);

            if (_selectedContainerEntry != null && _selectedContainerEntry.selectSideData != null)
            {
                _selectedModuleData = _selectedContainerEntry.selectSideData.moduleData;
            }
            else
            {
                _selectedModuleData = null;
                Debug.LogWarning("ContainerEntry or its selectSideData is missing.");
            }
        }


        private void ResetSelection()
        {
            _selectedContainer = null;
            _selectedContainerEntry = null;
            _selectedSide = default;
            _sideStates.Clear();
        }


        private Dictionary<SideType, GameObject> FindAllStatesOnContainer(GameObject selectedContainer)
        {
            var dictionary = new Dictionary<SideType, GameObject>();

            if (selectedContainer == null)
            {
                return dictionary;
            }

            int childCount = selectedContainer.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var childTransform = selectedContainer.transform.GetChild(i);

                if (childTransform.TryGetComponent<SideIdentifier>(out var sideIdentifier))
                {
                    if (childTransform.childCount > 0)
                    {
                        if (!dictionary.ContainsKey(sideIdentifier.sideType))
                        {
                            dictionary.Add(sideIdentifier.sideType, childTransform.GetChild(0).gameObject);
                        }
                    }
                }
            }

            return dictionary;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            DrawContainerSelection();

            if (_selectedContainer != null)
            {
                DrawMaterialSelection();
                DrawWearLevelControls();
                DrawSideSelection();
            }
            else
            {
                EditorGUILayout.HelpBox("Please select a container object in the scene.", MessageType.Info);
            }
        }

        #region Container Selection

        private void DrawContainerSelection()
        {
            GUILayout.Label("Size:", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (containerData == null || containerData.containers == null)
            {
                EditorGUILayout.HelpBox("Container data is not assigned or empty. Please check your data asset.",
                    MessageType.Warning);
                return;
            }

            GUILayout.BeginHorizontal();

            foreach (var container in containerData.containers)
            {
                bool isSelected = _selectedContainerEntry == container;

                GUI.backgroundColor = Color.clear;
                Rect buttonRect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64), GUILayout.Height(64));
                if (GUI.Button(buttonRect, new GUIContent(container.icon, container.name.ToString())))
                {
                    _selectedContainerEntry = container;
                    if (_selectedContainer != null)
                    {
                        _selectedContainer.name = container.name.ToString();
                        _selectedContainerEntry = container;
                    }
                }


                if (isSelected && _pressedTexture != null)
                {
                    GUI.DrawTexture(buttonRect, _pressedTexture, ScaleMode.StretchToFill);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (_selectedContainerEntry != null)
            {
                GUI.backgroundColor = Color.grey;
                if (GUILayout.Button("Create New", GUILayout.Width(80), GUILayout.Height(25)))
                {
                    SpawnContainer(_selectedContainerEntry);
                }

                GUI.backgroundColor = Color.white;
            }

        }

        private void SpawnContainer(ContainerEntry entry)
        {
            if (entry.prefab == null)
            {
                Debug.LogWarning($"Prefab for {entry.name} is not assigned in ContainerData.");
                return;
            }

            Vector3 spawnPosition = Vector3.zero;
            if (_selectedContainer != null)
            {
                spawnPosition = _selectedContainer.transform.position;
            }

            GameObject newContainer = (GameObject)PrefabUtility.InstantiatePrefab(entry.prefab);
            newContainer.transform.position = spawnPosition;
            newContainer.tag = "Container";

            GameObject containerParent = GameObject.Find("Containers");
            if (containerParent == null)
            {
                containerParent = new GameObject("Containers");
            }
            newContainer.transform.SetParent(containerParent.transform);

            _selectedContainer = newContainer;
            _selectedContainerEntry = entry;

            EditorApplication.delayCall += () =>
            {
                Selection.activeGameObject = newContainer;
            };
        }



        private ContainerEntry FindContainerEntryByType(GameObject containerObject)
        {
            var typeIdentifier = containerObject.GetComponent<ContainerTypeIdentifier>();
            if (typeIdentifier == null)
            {
                Debug.LogWarning("ContainerTypeIdentifier component not found on the selected container.");
                return null;
            }

            foreach (var entry in containerData.containers)
            {
                if (entry.name == typeIdentifier.containerType)
                {
                    return entry;
                }
            }

            Debug.LogWarning($"No ContainerEntry found for type: {typeIdentifier.containerType}");
            return null;
        }

        #endregion

        #region Side Selection

        private void DrawSideSelection()
        {
            GUILayout.Space(5);
            GUILayout.Label("Type:", EditorStyles.boldLabel);
            GUILayout.Space(5);
            GUI.backgroundColor = Color.clear;

            if (_selectedContainerEntry == null || _selectedContainerEntry.selectSideData == null)
            {
                EditorGUILayout.HelpBox("No side data available for the selected container.", MessageType.Warning);
                return;
            }

            GUILayout.BeginVertical();
            int columnCount = 4;
            int currentColumn = 0;

            GUILayout.BeginHorizontal();

            foreach (var side in _selectedContainerEntry.selectSideData.sides)
            {
                bool isSelected = _selectedSide == side.sideType;
                bool isRemoved = !_sideStates.ContainsKey(side.sideType);

                Color originalColor = GUI.color;
                bool isFrontWithoutDoor = false;
                if (side.sideType == SideType.Front && _sideStates.TryGetValue(SideType.Front, out var frontModule))
                {
                    isFrontWithoutDoor = frontModule.GetComponentInChildren<FrontDoorIdentifier>() == null;
                }
                if (side.sideType == SideType.Front && isFrontWithoutDoor)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                }
                else if (isRemoved)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                }
                else
                {
                    GUI.color = Color.white;
                }

                Rect buttonRect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64), GUILayout.Height(64));
                if (GUI.Button(buttonRect, new GUIContent(side.icon, side.sideType.ToString())))
                {
                    _selectedSide = side.sideType;
                    _selectedModuleData = _selectedContainerEntry.selectSideData.moduleData;
                }

                if (isSelected && _pressedTexture != null)
                {
                    GUI.DrawTexture(buttonRect, _pressedTexture, ScaleMode.ScaleToFit);
                }

                GUI.color = originalColor;

                currentColumn++;
                if (currentColumn >= columnCount)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    currentColumn = 0;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            DrawOptionStackModules();
            GUILayout.Space(10);
            if (_selectedModuleData != null)
            {
                DrawModuleSelection();
            }
        }


        #endregion

        #region OptionStackModule Logic

        private void DrawOptionStackModules()
        {
            RailingsData railingsData = GetCurrentRailingsData();

            GUILayout.Label("Additional Options:", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if ((_selectedContainerEntry == null || _selectedContainerEntry.optionStackModules == null) &&
                (_selectedContainerEntry == null || _selectedContainerEntry.railingsData == null))
            {
                EditorGUILayout.HelpBox("No additional options or railings available for the selected container.",
                    MessageType.Info);
                return;
            }

            int columnCount = 4;
            int currentColumn = 0;

            GUILayout.BeginVertical();

            if (_selectedContainerEntry != null && _selectedContainerEntry.optionStackModules != null)
            {
                GUILayout.BeginHorizontal();
                foreach (var option in _selectedContainerEntry.optionStackModules)
                {
                    bool isPlaced = IsOptionPlaced(option);

                    Rect buttonRect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64), GUILayout.Height(64));
                    if (GUI.Button(buttonRect, new GUIContent(option.icon, option.name)))
                    {
                        if (isPlaced)
                        {
                            RemoveOption(option);
                        }
                        else
                        {
                            PlaceOption(option);
                        }
                    }

                    if (isPlaced && _pressedTexture != null)
                    {
                        GUI.DrawTexture(buttonRect, _pressedTexture, ScaleMode.StretchToFill);
                    }

                    currentColumn++;
                    if (currentColumn >= columnCount)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        currentColumn = 0;
                    }
                }

                if (_selectedContainerEntry != null && _selectedContainerEntry.railingsData != null &&
                    _selectedContainerEntry.railingsData.railings.Exists(r => r.side == _selectedSide))
                {
                    Texture2D railingsIcon = _selectedContainerEntry.railingsIcon;
                    bool isRailingsActive = false;

                    if (_selectedContainer != null)
                    {
                        var railingsIdentifier = _selectedContainer.GetComponent<RailingsIdentifier>();
                        if (railingsIdentifier != null)
                        {
                            isRailingsActive = railingsIdentifier.IsRailingsActive(_selectedSide);
                        }
                    }

                    Rect railingsButtonRect =
                        GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64), GUILayout.Height(64));
                    if (GUI.Button(railingsButtonRect, new GUIContent(railingsIcon, "Toggle Railings")))
                    {
                        ToggleRailings(_selectedSide);
                    }

                    if (isRailingsActive && _pressedTexture != null)
                    {
                        GUI.DrawTexture(railingsButtonRect, _pressedTexture, ScaleMode.StretchToFill);
                    }
                }

                if (currentColumn > 0)
                {
                    GUILayout.EndHorizontal();
                }
            }


            if (_selectedContainerEntry.floorStates != null && _selectedContainerEntry.floorStates.Count > 0)
            {
                int currentFloorState =
                    _selectedContainerEntry.floorStates.FindIndex(state => IsFloorStateActive(state));
                currentFloorState = (currentFloorState + 1) % _selectedContainerEntry.floorStates.Count;

                GUILayout.Label("Floor State:", EditorStyles.boldLabel);
                GUILayout.Space(5);

                Rect floorButtonRect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64), GUILayout.Height(64));
                var nextFloorState = _selectedContainerEntry.floorStates[currentFloorState];

                if (GUI.Button(floorButtonRect, new GUIContent(nextFloorState.icon, "Toggle Floor State")))
                {
                    ToggleFloorState(_selectedContainerEntry.floorStates[currentFloorState]);
                }
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.Label("Props Placement:", EditorStyles.boldLabel);
            GUILayout.Space(5);

            bool isPropsPlacementEnabled = (_selectedSide == SideType.Side_R1 || _selectedSide == SideType.Side_R2 ||
                                            _selectedSide == SideType.Side_L1 || _selectedSide == SideType.Side_L2 ||
                                            _selectedSide == SideType.Front || _selectedSide == SideType.Back) &&
                                           _hasWallHandler && _sideStates.ContainsKey(_selectedSide);

            if (!isPropsPlacementEnabled)
            {
                GUI.color = new Color(1, 1, 1, 0.2f);
                GUI.enabled = false;
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            currentColumn = 0;
            if (_wallHandler != null)
            {
                for (int i = 0; i < _decalIcons.Length; i++)
                {
                    bool isActive = _wallHandler != null && _wallHandler.IsDecalActive(i);
                    GUI.backgroundColor = isActive ? Color.gray : Color.white;
                    bool isDecalSelectable = _wallHandler != null && _wallHandler.GetAvailableDecals().Contains(i);
                    GUI.enabled = _selectedContainer != null && isDecalSelectable;

                    Rect buttonRect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64), GUILayout.Height(64));
                    if (GUI.Button(buttonRect, new GUIContent(_decalIcons[i], "Decal " + (i + 1))))
                    {
                        _wallHandler.ToggleDecal(i, _materialSets[_selectedMaterialIndex], _wornLevel, _enableWornLevel);
                        AutoSaveContainer();
                    }

                    if (isActive && _pressedTexture != null)
                    {
                        GUI.DrawTexture(buttonRect, _pressedTexture, ScaleMode.StretchToFill);
                    }
                }

                currentColumn++;
                if (currentColumn >= columnCount)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    currentColumn = 0;
                }
            }

            if (currentColumn > 0)
            {
                while (currentColumn < columnCount)
                {
                    GUILayout.Label("", GUILayout.Width(64), GUILayout.Height(64));
                    currentColumn++;
                }

            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUI.enabled = true;
            GUI.color = Color.white;
        }


        private RailingsData GetCurrentRailingsData()
        {
            return _selectedContainerEntry?.railingsData;
        }


        private void ToggleRailings(SideType side)
        {
            RailingsData railingsData = GetCurrentRailingsData();
            if (_selectedContainer == null || railingsData == null)
            {
                Debug.LogWarning("No container or railings data found.");
                return;
            }

            var railingsIdentifier = _selectedContainer.GetComponent<RailingsIdentifier>();

            if (railingsIdentifier == null)
            {
                railingsIdentifier = _selectedContainer.AddComponent<RailingsIdentifier>();
            }

            var railingEntry = railingsData.railings.Find(r => r.side == side);
            if (railingEntry != null)
            {
                Material selectedMaterial = _materialSets[_selectedMaterialIndex].FrameMaterial;
                railingsIdentifier.ToggleRailings(side, railingEntry.prefab, selectedMaterial);
            }

        }


        private void ToggleFloorState(FloorToggleState newState)
        {
            if (_selectedContainer == null)
            {
                Debug.LogWarning("No container selected to toggle floor state.");
                return;
            }

            foreach (var floorState in _selectedContainerEntry.floorStates)
            {
                Transform existingFloor = _selectedContainer.transform.Find(floorState.prefab.name);
                if (existingFloor != null)
                {
                    DestroyImmediate(existingFloor.gameObject);
                }
            }

            if (newState.prefab != null)
            {
                GameObject newFloor = Instantiate(newState.prefab, _selectedContainer.transform);
                newFloor.name = newState.prefab.name;

                newFloor.transform.localPosition = newState.localPosition;

                ApplyMaterialToNewModule(newFloor);
            }
            else
            {
                Debug.LogWarning("New state prefab is null. Cannot toggle floor state.");
            }
        }


        private bool IsFloorStateActive(FloorToggleState state)
        {
            if (_selectedContainer == null)
                return false;

            Transform floorTransform = _selectedContainer.transform.Find(state.prefab.name);
            return floorTransform != null && floorTransform.gameObject.activeSelf;
        }

        private bool IsOptionPlaced(OptionStackModule option)
        {
            if (_selectedContainer == null || option.prefab == null) return false;

            foreach (Transform child in _selectedContainer.transform)
            {
                if (child.name == option.prefab.name) return true;
            }

            return false;
        }

        private void PlaceOption(OptionStackModule option)
        {
            if (_selectedContainer == null || option.prefab == null)
            {
                Debug.LogWarning("Cannot place option. Container or option prefab is missing.");
                return;
            }

            GameObject newOption =
                (GameObject)PrefabUtility.InstantiatePrefab(option.prefab, _selectedContainer.transform);
            newOption.name = option.prefab.name;
            newOption.transform.localPosition = option.defaultPosition;
            newOption.transform.localRotation = option.defaultRotation;
            ApplyMaterialToNewModule(newOption);
            AutoSaveContainer();
            Repaint();
        }

        private void RemoveOption(OptionStackModule option)
        {
            if (_selectedContainer == null || option.prefab == null)
            {
                Debug.LogWarning("Cannot remove option. Container or option prefab is missing.");
                return;
            }

            foreach (Transform child in _selectedContainer.transform)
            {
                if (child.name == option.prefab.name)
                {
                    DestroyImmediate(child.gameObject);
                    AutoSaveContainer();
                    Repaint();
                    return;
                }
            }
        }

        #endregion

        #region Module Selection

        private void DrawModuleSelection()
        {
            GUILayout.Label("Objects To Set:", EditorStyles.boldLabel);
            GUILayout.Space(5);
            GUILayout.Space(10);
            GUI.backgroundColor = Color.grey;

            if (GUILayout.Button("Delete", GUILayout.Width(80), GUILayout.Height(25)))
            {
                DeleteModuleFromSide(_selectedSide);
            }

            GUI.backgroundColor = Color.white;

            GameObject currentModule = _sideStates.ContainsKey(_selectedSide) ? _sideStates[_selectedSide] : null;

            var correctModules = _selectedModuleData.modules
                .Where(x => x.sideTypes != null && x.sideTypes.Contains(_selectedSide))
                .ToList();

            if (!correctModules.Any())
            {
                EditorGUILayout.HelpBox("No modules match the selected side.", MessageType.Info);
                return;
            }

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(150));
            GUILayout.BeginVertical();

            int columnCount = 4;
            int currentColumn = 0;

            GUILayout.BeginHorizontal();

            foreach (var module in correctModules)
            {
                bool isCurrentModule = currentModule != null && currentModule.name == module.prefab.name;

                Rect buttonRect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64), GUILayout.Height(64));

                if (GUI.Button(buttonRect, new GUIContent(module.icon)))
                {
                    ReplaceModule(module, _selectedSide);

                }

                if (isCurrentModule && _pressedTexture != null)
                {
                    GUI.DrawTexture(buttonRect, _pressedTexture, ScaleMode.StretchToFill);
                }

                currentColumn++;
                if (currentColumn >= columnCount)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    currentColumn = 0;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }


        private void DeleteModuleFromSide(SideType sideType)
        {
            if (sideType == SideType.Front)
            {
                var module = _sideStates[sideType];
                FrontDoorIdentifier[] doors = FindObjectsOfType<FrontDoorIdentifier>();

                foreach (var door in doors)
                {
                    if (door.transform.IsChildOf(module.transform))
                    {
                        DestroyImmediate(door.gameObject);
                        Debug.Log($"Doors on {sideType} have been deleted.");
                    }
                }
            }
            else
            {
                if (_sideStates.ContainsKey(sideType))
                {
                    var moduleToRemove = _sideStates[sideType];
                    DestroyImmediate(moduleToRemove);
                    _sideStates.Remove(sideType);
                    switch (sideType)
                    {
                        case SideType.Side_L1:
                        case SideType.Side_L2:
                            UpdatePillarState(SideType.Side_L1, SideType.Side_L2);
                            break;
                        case SideType.Side_R1:
                        case SideType.Side_R2:
                            UpdatePillarState(SideType.Side_R1, SideType.Side_R2);
                            break;
                        case SideType.Top1:
                        case SideType.Top2:
                            UpdatePillarState(SideType.Top1, SideType.Top2);
                            break;
                    }

                    Transform sideTransform = FindSideTransform(_selectedContainer, sideType);
                    if (sideTransform != null)
                    {
                        Selection.activeGameObject = sideTransform.gameObject;
                    }

                    Repaint();
                }
            }
        }

        private void UpdatePillarState(SideType side, SideType oppositeSide)
        {
            bool hasSide = _sideStates.ContainsKey(side);
            bool hasOpposite = _sideStates.ContainsKey(oppositeSide);

            var pillar = _pillarIdentifiers.FirstOrDefault(x => x.SideTypes.Contains(side) || x.SideTypes.Contains(oppositeSide));
            if (pillar != null)
            {
                pillar.SetActive(hasSide || hasOpposite);
            }
        }


        private void ReplaceModule(SideModuleData.ModuleEntry module, SideType targetSide)
        {
            if (_selectedContainer == null || module.prefab == null)
            {
                return;
            }

            var occupiedSides = module.occupiedSides != null && module.occupiedSides.Count > 0
                ? module.occupiedSides
                : null;

            if (occupiedSides != null)
            {
                var needPillar = _pillarIdentifiers.FirstOrDefault(x => x.SideTypes.Contains(targetSide));
                needPillar?.SetActive(false);

                foreach (var side in occupiedSides)
                {
                    if (_sideStates.ContainsKey(side))
                    {
                        var existingModule = _sideStates[side];
                        DestroyImmediate(existingModule);
                        _sideStates.Remove(side);
                    }
                }
            }

            if (occupiedSides == null)
            {
                var needPillar = _pillarIdentifiers.FirstOrDefault(x => x.SideTypes.Contains(targetSide));
                needPillar?.SetActive(true);
            }

            if (_sideStates.TryGetValue(targetSide, out var state))
            {
                DestroyImmediate(state.gameObject);
                _sideStates.Remove(targetSide);
            }
            else
            {
                if (_sideTypesPair.ContainsKey(targetSide) &&
                    _sideStates.TryGetValue(_sideTypesPair[targetSide], out var pairedState))
                {
                    DestroyImmediate(pairedState.gameObject);
                    _sideStates.Remove(_sideTypesPair[targetSide]);
                }
            }

            Transform sideTransform = FindSideTransform(_selectedContainer, targetSide);
            if (sideTransform != null)
            {
                GameObject newModule = (GameObject)PrefabUtility.InstantiatePrefab(module.prefab, sideTransform);
                newModule.transform.localPosition = Vector3.zero;
                newModule.transform.localRotation = Quaternion.identity;
                _originalMaterials.Remove(_selectedContainer);


                ApplyMaterialToNewModule(newModule);

                _sideStates[targetSide] = newModule;
                Selection.activeGameObject = newModule;
            }
            


            Repaint();
        }

        private void ApplyMaterialToNewModule(GameObject module)
{
    if (module == null) return;

    var renderers = module.GetComponentsInChildren<Renderer>(true);
    foreach (var renderer in renderers)
    {
        Material[] currentMaterials = renderer.sharedMaterials.ToArray();

        for (int i = 0; i < currentMaterials.Length; i++)
        {
            Material originalMaterial = currentMaterials[i];
            if (originalMaterial == null) continue;

            string matName = originalMaterial.name;
            Material materialFromSet = null;
            bool matched = true;

            if (matName.StartsWith("MI_Container_"))
                materialFromSet = _materialSets[_selectedMaterialIndex].FrameMaterial;
            else if (matName.StartsWith("Details_Conteiners_"))
                materialFromSet = _materialSets[_selectedMaterialIndex].DetailsMaterial;
            else if (matName.StartsWith("MI_Valves_Paint"))
                materialFromSet = _materialSets[_selectedMaterialIndex].ValvesMaterial;
            else if (matName.StartsWith("MI_WoodFloor_B"))
                materialFromSet = _materialSets[_selectedMaterialIndex].FloorMaterial;
            else if (matName.StartsWith("MI_Conteiners_Paint-A_Grey"))
                materialFromSet = _materialSets[_selectedMaterialIndex].WallInsideMaterial;
            else if (matName.StartsWith("MI_Floor_Paint"))
                materialFromSet = _materialSets[_selectedMaterialIndex].StairsMaterial;
            else if (matName.StartsWith("MI_Blend_Conteiners_Metal-A"))
                materialFromSet = _materialSets[_selectedMaterialIndex].DoorMaterial;
            else
                matched = false;

            Material newMaterial;
            if (matched)
            {
                if (_enableWornLevel)
                {
                    newMaterial = new Material(materialFromSet);
                    newMaterial.SetFloat("_Worn_Level", _wornLevel);
                }
                else
                {
                    newMaterial = materialFromSet;
                }
            }
            else
            {
                newMaterial = _enableWornLevel ? new Material(originalMaterial) : originalMaterial;
            }

            currentMaterials[i] = newMaterial;
        }

        renderer.sharedMaterials = currentMaterials;
    }
}



        private Transform FindSideTransform(GameObject container, SideType sideType)
        {
            foreach (Transform child in container.transform)
            {
                var identifier = child.GetComponent<SideIdentifier>();
                if (identifier != null && identifier.sideType == sideType)
                {
                    return child;
                }
            }

            Debug.LogWarning($"Side {sideType} not found in the container.");
            return null;
        }

        private void AutoSaveContainer()
        {
            if (_selectedContainer == null) return;

            EditorUtility.SetDirty(_selectedContainer);

            foreach (var renderer in _selectedContainer.GetComponentsInChildren<Renderer>())
            {
                EditorUtility.SetDirty(renderer);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

        #endregion
    }
}