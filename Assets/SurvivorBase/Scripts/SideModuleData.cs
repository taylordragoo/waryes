using System.Collections.Generic;
using UnityEngine;

namespace SurvivorBase.Scripts
{
    [CreateAssetMenu(fileName = "SideModuleData", menuName = "Custom Data/Side Module Data")]
    public class SideModuleData : ScriptableObject
    {
        public List<ModuleEntry> modules;

        [System.Serializable]
        public class ModuleEntry
        {
            public List<SideType> sideTypes; 
            public List<SideType> occupiedSides; 
            public Texture2D icon;
            public GameObject prefab;
        }
    }
}