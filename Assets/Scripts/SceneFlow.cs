using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlow : MonoBehaviour
{
    public void ChangeToMain()
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void ChangeToOptions()
    {
        SceneManager.LoadScene("SetAtributes");
    }
    public void ChangeToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void ChangeGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}
