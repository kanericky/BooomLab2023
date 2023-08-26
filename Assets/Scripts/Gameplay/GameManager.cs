using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private bool _isFullScreen = false;

        private void Start()
        {
            Instance = this;
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(1920 / 2, 1080 / 2, FullScreenMode.Windowed);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!_isFullScreen)
                {
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                }
                else if (_isFullScreen)
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    Screen.SetResolution(1920 / 2, 1080 / 2, FullScreenMode.Windowed);
                }

                _isFullScreen = !_isFullScreen;
            }
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
