using System.Collections.Generic;
using UnityEngine;

namespace SurvivorBase.Scripts
{
    [CreateAssetMenu(fileName = "SelectSideData", menuName = "Custom Data/Select Side Data")]
    public class SelectSideData : ScriptableObject
    {
        public List<SideEntry> sides;
        public SideModuleData moduleData;
    }

    [System.Serializable]
    public class SideEntry
    {
        public SideType sideType;     
        public Texture2D icon;
    
    
    }

    public enum SideType
    {
        Side_L1,
        Side_L2,
        Side_R1,
        Side_R2,
        Front,
        Back,
        Top1,
        Top2
    }
}