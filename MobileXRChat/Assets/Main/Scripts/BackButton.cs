using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HandDVR
{
    public class BackButton : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClick();
            }
        }

        public void OnClick()
        {
            StartCoroutine(loadScene());
        }

        IEnumerator loadScene()
        {
            yield return SceneManager.LoadSceneAsync("Menu");

            if (SceneManager.GetActiveScene().name != "Menu")
            {
                Application.Quit();
            }
        }
    }
}
