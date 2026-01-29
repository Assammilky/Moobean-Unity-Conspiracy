using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuScript : MonoBehaviour
{
    

    public string play_scene;

    public void Play_Button()
    {

        SceneManager.LoadScene(play_scene); 

    }

    public void Open_Settings(){

    



    }

    public void Quit(){

        Application.Quit();

    }


}
