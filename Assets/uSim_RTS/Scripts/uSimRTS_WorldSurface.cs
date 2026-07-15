using UnityEngine;

namespace uSimRTS
{
    internal static class uSimRTS_WorldSurface
    {
        const float MinimumGroundNormalY = 0.5f;

        public static bool IsWalkable(RaycastHit hit)
        {
            Collider collider = hit.collider;

            if (collider == null || collider.isTrigger)
                return false;

            if (collider.GetComponentInParent<uSimRTS_Unit>() != null ||
                collider.GetComponentInParent<uSimRTS_BaseBuilding>() != null)
            {
                return false;
            }

            if (!collider.CompareTag("world") && !collider.CompareTag("Untagged"))
                return false;

            if (IsWater(collider))
                return false;

            return hit.normal.y >= MinimumGroundNormalY;
        }

        public static bool IsWater(Collider collider)
        {
            return collider != null &&
                collider.name.IndexOf("water", System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool IsTerrainSurface(Collider collider)
        {
            return collider != null &&
                (collider is TerrainCollider || collider.CompareTag("world"));
        }

        public static bool IsRoadSupport(Collider collider)
        {
            if (collider == null)
                return false;

            if (collider.GetComponent<JBooth.MicroVerseCore.RoadMaterialOverride>() != null)
                return true;

            string surfaceName = collider.name;

            if (surfaceName.StartsWith("Parking Space", System.StringComparison.OrdinalIgnoreCase))
                return true;

            if (surfaceName.IndexOf("Concrete Bridge 2 Lanes", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                surfaceName.IndexOf("Tunnel 2 Lanes", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (surfaceName.StartsWith("Merge ", System.StringComparison.OrdinalIgnoreCase) ||
                surfaceName.StartsWith("Highway Ramp", System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return surfaceName.StartsWith("Road ", System.StringComparison.OrdinalIgnoreCase) &&
                surfaceName.IndexOf("Block", System.StringComparison.OrdinalIgnoreCase) < 0;
        }
    }
}
