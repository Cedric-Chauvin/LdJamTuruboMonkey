using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    private void Awake()
    {
        Time.timeScale = 1.0f;
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
