using System.Collections.Generic;
using UnityEngine;

namespace SurvivorBase.Scripts
{
    [CreateAssetMenu(fileName = "ContainerData", menuName = "Custom Data/Container Data")]
    public class ContainerData : ScriptableObject
    {
        public List<ContainerEntry> containers;
    }

    [System.Serializable]
    public class ContainerEntry
    {
        public ContainerType name;
        public Texture2D icon;
        public GameObject prefab;
        public SelectSideData selectSideData;
        public List<OptionStackModule> optionStackModules;
        public List<FloorToggleState> floorStates;
        public RailingsData railingsData; 
        public Texture2D railingsIcon; 
    }

    public enum ContainerType
    {
        TypeA,
        TypeB,
        TypeC
    }
}