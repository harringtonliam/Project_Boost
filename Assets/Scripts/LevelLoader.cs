using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour


{
    //Parameters
    [SerializeField] float levelLoadDelaySeconds = 1.5f;
    [SerializeField] Text levelText;


    //member variables
    int sceneToLoad;

    // Start is called before the first frame update
    void Start()
    {
        int maxSceneBuildIndex = SceneManager.sceneCountInBuildSettings-1;
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.buildIndex > 0 && currentScene.buildIndex < maxSceneBuildIndex)
        {
            levelText.text = "Level: " + SceneManager.GetActiveScene().buildIndex.ToString();
        }
        else
        {
            levelText.text = string.Empty;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }

    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();


        if (currentScene.buildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {

            sceneToLoad = currentScene.buildIndex + 1;
        }
        else
        {
            sceneToLoad = 0;
        }
        StartCoroutine("LoadSceneWithDelay"); 

    }

    public void LoadStartScene()
    {
        Debug.Log("Level Loader LoadStartScene");
        SceneManager.LoadScene(0);
    }

    public void LoadLevel1()
    {
        sceneToLoad = 1; //Level 1 scene is always at index 1
        StartCoroutine("LoadSceneWithDelay");

    }

    public void LoadCurrentLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        sceneToLoad = currentScene.buildIndex;
        StartCoroutine("LoadSceneWithDelay");
    }


    private IEnumerator LoadSceneWithDelay()
    {

        yield return new WaitForSeconds(levelLoadDelaySeconds);
        Debug.Log("LoadSceneWithDelay " + sceneToLoad.ToString());
        SceneManager.LoadScene(sceneToLoad);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
