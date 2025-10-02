using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnLevel1Click()
    {
        SceneManager.LoadScene((int)SceneList.Level1);
    }

    public void OnLevel2Click()
    {
        SceneManager.LoadScene((int)SceneList.Level2);
    }
}
