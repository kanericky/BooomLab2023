using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private void Start()
        {
            Instance = this;
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(1920 / 2, 1080 / 2, FullScreenMode.Windowed);
        }

        public void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        
    }

}
