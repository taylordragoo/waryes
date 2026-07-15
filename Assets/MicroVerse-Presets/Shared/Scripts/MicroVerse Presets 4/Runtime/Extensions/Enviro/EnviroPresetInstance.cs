#if ENVIRO_3

using JBooth.MicroVerseCore.Browser;
using UnityEngine;

namespace Rowlan.Biomes_Presets_4
{
    /// <summary>
    /// Instance type handling of presets specific for Enviro 3.
    /// Configuration is loaded into enviro 3 and then the gameobject is destroyed
    /// 
    /// </summary>
    public class EnviroPresetInstance : MonoBehaviour, IContentBrowserDropAction
    {
        /// <summary>
        /// The enviro configuration to load
        /// </summary>
        public Enviro.EnviroConfiguration configuration;

        #region ContentBrowser
        /// <summary>
        /// Execute an action after this prefab was dropped into the scene from the content browser
        /// </summary>
        /// <param name="destroyAfterExecute"></param>
        public void Execute(out bool destroyAfterExecute)
        {
            destroyAfterExecute = true;

            if (Enviro.EnviroManager.instance == null)
            {
                Debug.Log("Enviro manager instance not found. Please add one to the scene");
                return;
            }

            if (configuration == null)
            {
                Debug.Log("Missing configuration");
                return;
            }

            Enviro.EnviroManager.instance.configuration = configuration;
            Enviro.EnviroManager.instance.LoadConfiguration();

            Debug.Log($"Configuration {configuration} loaded");
        }
        #endregion ContentBrowser
    }
}
#endif