using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace uSimRTS
{
    public class uSimRTS_Main : MonoBehaviour
    {
        [Tooltip("First mission scene index.")]
        public int initialSceneIndex;
        [Tooltip("Player's current mission.(saved to player prefs)")]
        public int currentLvl;
        [Tooltip("The current instance of this object.(it's set on start)")]
        public static uSimRTS_Main instance;

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this);
            instance = this;
            SceneManager.LoadScene(1);
        }


        private void OnLevelWasLoaded(int level)
        {
            loading = false;
            if (GameObject.FindObjectOfType<uSimRTS_GameOverOptions>() != null)
                GameObject.FindObjectOfType<uSimRTS_GameOverOptions>().check = true;
        }


        public void StartNewGame()
        {
            SceneManager.LoadScene(initialSceneIndex);
        }

       
        public void SaveGame()
        {
            currentLvl = SceneManager.GetActiveScene().buildIndex;
            PlayerPrefs.SetInt("uSimRTS_playerLevel", currentLvl );

        }

        public void LoadGame()
        {
            currentLvl = PlayerPrefs.GetInt("uSimRTS_playerLevel");
            SceneManager.LoadScene(currentLvl);
        }

        public void LoadNextLevel()
        {
            if (loading)
                return;

            loading = true;

            currentLvl++;
            if (currentLvl < SceneManager.sceneCountInBuildSettings)
                WaitAndLoadScene(currentLvl);
            else
                ReturnToMain();
        }

        public void WaitAndLoadScene(int scene)
        {
            StartCoroutine(DoWaitAndLoadScene(scene));
        }
        bool loading;
        IEnumerator DoWaitAndLoadScene(int scene)
        {            
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene(scene);
            currentLvl = scene;

        }

        public void ReturnToMain()
        {
            SceneManager.LoadScene(0);
        }
    }
}

