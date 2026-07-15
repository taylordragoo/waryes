using System.Collections.Generic;
using UnityEngine;

namespace SurvivorBase.Scripts
{
    [CreateAssetMenu(fileName = "RailingsData", menuName = "Custom Data/Railings Data")]
    public class RailingsData : ScriptableObject
    {
        public List<RailingEntry> railings;

        [System.Serializable]
        public class RailingEntry
        {
            public SideType side;
            public GameObject prefab;
        }
    }
}