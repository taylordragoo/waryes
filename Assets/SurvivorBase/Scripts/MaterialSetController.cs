using System.Linq;
using UnityEngine;

namespace SurvivorBase.Scripts
{
    [ExecuteAlways]
    public class MaterialSetController : MonoBehaviour
    {
        [SerializeField] private bool enableWornLevel = false;
        [Range(0f, 2f)] [SerializeField] private float wornLevel = 0f;
        [Range(0f, 4f)] [SerializeField] private int materialSetIndex = 0;

        private ContainerMaterialSet[] materialSets;
        private Material[][] originalMaterials;
        private Renderer[] renderers;

        private readonly string[] allowedPrefixes = new[]
        {
            "MI_Container_", "Details_Conteiners_", "MI_Valves_Paint",
            "MI_WoodFloor_B", "MI_Floor_Paint", "MI_Conteiners_Paint-A_Grey", "MI_Blend_Conteiners_Metal-A"
        };

        private void OnEnable()
        {
            materialSets = Resources.LoadAll<ContainerMaterialSet>("Materials");
            renderers = GetComponentsInChildren<Renderer>(true);
            originalMaterials = renderers.Select(r => r.sharedMaterials.ToArray()).ToArray();
            ApplyMaterials();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying && materialSets != null && renderers != null)
            {
                ApplyMaterials();
            }
        }

        private void ApplyMaterials()
        {
            if (materialSets == null || materialSetIndex >= materialSets.Length) return;

            var set = materialSets[materialSetIndex];

            for (int r = 0; r < renderers.Length; r++)
            {
                var mats = originalMaterials[r];
                Material[] newMats = new Material[mats.Length];

                for (int i = 0; i < mats.Length; i++)
                {
                    Material mat = mats[i];
                    if (mat == null)
                    {
                        newMats[i] = null;
                        continue;
                    }

                    string name = mat.name;
                    Material baseMat = GetMaterialFromSet(name, set);

                    if (enableWornLevel && baseMat != null)
                    {
                        Material instance = new Material(baseMat);
                        instance.SetFloat("_Worn_Level", wornLevel);
                        newMats[i] = instance;
                    }
                    else
                    {
                        newMats[i] = baseMat ?? mat;
                    }
                }

                renderers[r].sharedMaterials = newMats;
            }
        }

        private Material GetMaterialFromSet(string name, ContainerMaterialSet set)
        {
            if (name.StartsWith("MI_Container_")) return set.FrameMaterial;
            if (name.StartsWith("Details_Conteiners_")) return set.DetailsMaterial;
            if (name.StartsWith("MI_Valves_Paint")) return set.ValvesMaterial;
            if (name.StartsWith("MI_WoodFloor_B")) return set.FloorMaterial;
            if (name.StartsWith("MI_Conteiners_Paint-A_Grey")) return set.WallInsideMaterial;
            if (name.StartsWith("MI_Floor_Paint")) return set.StairsMaterial;
            if (name.StartsWith("MI_Blend_Conteiners_Metal-A")) return set.DoorMaterial;
            return null;
        }
    }
}
