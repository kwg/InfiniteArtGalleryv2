using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {



    public void PlayTWEANNTest()
    {
        SceneManager.LoadScene("CPPNImageTest");
    }

    public void PlayEvolutionTest()
    {
        SceneManager.LoadScene("RoomTest_02");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
