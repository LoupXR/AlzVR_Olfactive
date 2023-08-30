using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartButton() {
        SceneManager.LoadScene("Kitchen");
        SceneManager.LoadScene("AlzVR", LoadSceneMode.Additive);
    }

    public void QuitButton() {
        Application.Quit();
    }
}
