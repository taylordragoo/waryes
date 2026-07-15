using UnityEngine;

namespace uSimRTS
{
    [DefaultExecutionOrder(10000)]
    public sealed class uSimRTS_SceneLightingSetup : MonoBehaviour
    {
        [SerializeField] private string decorativeLightName = "Lamp Light";

        private void Awake()
        {
            DisableDecorativeShadows();
        }

        private void Start()
        {
            DisableDecorativeShadows();
        }

        private void DisableDecorativeShadows()
        {
            Light[] lights = FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (Light sceneLight in lights)
            {
                if (sceneLight.name == decorativeLightName)
                    sceneLight.shadows = LightShadows.None;
            }
        }
    }
}
