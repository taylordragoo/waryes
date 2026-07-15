using UnityEngine;

namespace SurvivorBase.Scripts
{
    [System.Serializable]
    public class OptionStackModule
    {
        public string name;
        public GameObject prefab;
        public Texture icon;
        public Vector3 defaultPosition;
        public Quaternion defaultRotation;
    }

}