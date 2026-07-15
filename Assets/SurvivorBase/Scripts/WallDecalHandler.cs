using System.Collections.Generic;
using UnityEngine;

namespace SurvivorBase.Scripts
{
    public class WallDecalHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> decalPrefabs; 
        [SerializeField] private List<Vector3> spawnPositions; 

        private Dictionary<int, GameObject> _activeDecals = new Dictionary<int, GameObject>();
        private Dictionary<int, int> _decalMaterialIndex = new Dictionary<int, int>(); 
        
        public void ToggleDecal(int index, ContainerMaterialSet selectedSet, float wearLevel, bool useInstances)
        {
            if (index < 0 || index >= decalPrefabs.Count || decalPrefabs[index] == null) return;

            if (_activeDecals.ContainsKey(index))
            {
                DestroyImmediate(_activeDecals[index]);
                _activeDecals.Remove(index);
                return;
            }

            if (!_decalMaterialIndex.ContainsKey(index))
            {
                _decalMaterialIndex[index] = 0;
            }
            else
            {
                var materialHandler = decalPrefabs[index].GetComponent<DecalMaterialHandler>();
                if (materialHandler != null && materialHandler.decalMaterials.Length > 0)
                {
                    _decalMaterialIndex[index] = (_decalMaterialIndex[index] + 1) % materialHandler.decalMaterials.Length;
                }
                else
                {
                    _decalMaterialIndex[index] = 0;
                }
            }

            Vector3 spawnPosition = spawnPositions.Count > index ? spawnPositions[index] : transform.position;
            GameObject newDecal = Instantiate(decalPrefabs[index], transform);
            newDecal.transform.localPosition = spawnPosition;
            newDecal.transform.localRotation = Quaternion.identity;
            newDecal.transform.localScale = Vector3.one;

            _activeDecals[index] = newDecal;

            var newMaterialHandler = newDecal.GetComponent<DecalMaterialHandler>();
            if (newMaterialHandler != null)
            {
                newMaterialHandler.ApplyMaterial(_decalMaterialIndex[index]);
            }
            else if (useInstances)
            {
                ApplyMaterialToDecal(newDecal, selectedSet, wearLevel);
            }
            else
            {
                ApplySharedMaterialToDecal(newDecal, selectedSet);
            }
        }
        private void ApplySharedMaterialToDecal(GameObject decal, ContainerMaterialSet selectedSet)
        {
            if (decal == null || selectedSet == null) return;

            var renderers = decal.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                Material[] originalMats = renderer.sharedMaterials;
                Material[] updatedMats = new Material[originalMats.Length];

                for (int i = 0; i < originalMats.Length; i++)
                {
                    Material mat = originalMats[i];
                    if (mat == null)
                    {
                        updatedMats[i] = null;
                        continue;
                    }

                    string matName = mat.name;

                    if (matName.StartsWith("MI_Container_"))
                        updatedMats[i] = selectedSet.FrameMaterial;
                    else if (matName.StartsWith("Details_Conteiners_"))
                        updatedMats[i] = selectedSet.DetailsMaterial;
                    else if (matName.StartsWith("MI_Valves_Paint"))
                        updatedMats[i] = selectedSet.ValvesMaterial;
                    else
                        updatedMats[i] = mat;
                }

                renderer.sharedMaterials = updatedMats;
            }
        }


        public void RefreshDecalMaterials(Material selectedMaterial, float wearLevel, bool enableWornLevel)
        {
            foreach (var decal in _activeDecals.Values)
            {
                if (decal == null) continue;

                var renderers = decal.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers)
                {
                    if (renderer.sharedMaterials == null) continue;

                    Material[] currentMaterials = renderer.sharedMaterials;
                    Material[] updatedMaterials = new Material[currentMaterials.Length];

                    for (int i = 0; i < currentMaterials.Length; i++)
                    {
                        var currentMat = currentMaterials[i];
                        if (currentMat == null)
                        {
                            updatedMaterials[i] = null;
                            continue;
                        }

                        string matName = currentMat.name;

                        bool shouldReplace =
                            matName.StartsWith("MI_Container_") ||
                            matName.StartsWith("Details_Conteiners_") ||
                            matName.StartsWith("MI_Valves_Paint");

                        if (shouldReplace && enableWornLevel && selectedMaterial != null)
                        {
                            var newMat = new Material(selectedMaterial);
                            newMat.SetFloat("_Worn_Level", wearLevel);
                            updatedMaterials[i] = newMat;
                        }
                        else
                        {
                            updatedMaterials[i] = currentMat;
                        }

                    }

                    renderer.sharedMaterials = updatedMaterials;
                }
            }
        }


        public bool IsDecalActive(int index)
        {
            return _activeDecals.ContainsKey(index);
        }

        public List<GameObject> GetActiveDecals()
        {
            return new List<GameObject>(_activeDecals.Values);
        }
        public List<int> GetAvailableDecals()
        {
            List<int> availableDecals = new List<int>();
            for (int i = 0; i < decalPrefabs.Count; i++)
            {
                availableDecals.Add(i);
            }
            return availableDecals;
        }
        
        private void ApplyMaterialToDecal(GameObject decal, ContainerMaterialSet selectedSet, float wearLevel)
        {
            if (decal == null || selectedSet == null) return;

            var renderers = decal.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                Material[] originalMats = renderer.sharedMaterials;
                Material[] newMaterials = new Material[originalMats.Length];

                for (int i = 0; i < originalMats.Length; i++)
                {
                    Material originalMaterial = originalMats[i];
                    if (originalMaterial == null)
                    {
                        newMaterials[i] = null;
                        continue;
                    }

                    string matName = originalMaterial.name;
                    Material baseMat = null;

                    if (matName.StartsWith("MI_Container_"))
                        baseMat = selectedSet.FrameMaterial;
                    else if (matName.StartsWith("Details_Conteiners_"))
                        baseMat = selectedSet.DetailsMaterial;
                    else if (matName.StartsWith("MI_Valves_Paint"))
                        baseMat = selectedSet.ValvesMaterial;
                    else
                    {
                        // Матеріал не підходить — залишаємо як є
                        newMaterials[i] = originalMaterial;
                        continue;
                    }

                    // Створюємо інстанс лише для дозволених
                    Material newMat = new Material(baseMat);
                    newMat.SetFloat("_Worn_Level", wearLevel);
                    newMaterials[i] = newMat;
                }

                renderer.sharedMaterials = newMaterials;
            }
        }
    }
}