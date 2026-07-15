using System.Collections.Generic;
using UnityEngine;

namespace SurvivorBase.Scripts
{
    public class RailingsIdentifier : MonoBehaviour
    {
        [System.Serializable]
        public class RailingEntry
        {
            public SideType side;
            public GameObject railingObject;
        }

        public List<RailingEntry> activeRailings = new List<RailingEntry>();

        public void ToggleRailings(SideType side, GameObject prefab, Material material)
        {
            var entry = activeRailings.Find(r => r.side == side);

            if (entry != null)
            {
                bool newState = !entry.railingObject.activeSelf;
                DestroyImmediate(entry.railingObject);
                activeRailings.Remove(entry);
            }
            else
            {
                GameObject newRailing = Instantiate(prefab, transform);
                newRailing.name = $"{prefab.name}_{side}";

                ApplyMaterialToNewModule(newRailing, material);

                activeRailings.Add(new RailingEntry { side = side, railingObject = newRailing });
            }
        }

        public bool IsRailingsActive(SideType side)
        {
            var entry = activeRailings.Find(r => r.side == side);
            return entry != null && entry.railingObject.activeSelf;
        }

        public bool HasRailings(SideType side)
        {
            return activeRailings.Exists(r => r.side == side);
        }

        private void ApplyMaterialToNewModule(GameObject module, Material material)
        {
            if (module == null || material == null) return;

            var renderers = module.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                Material[] newMaterials = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = material;
                }
                renderer.sharedMaterials = newMaterials;
            }
        }
    }
}
