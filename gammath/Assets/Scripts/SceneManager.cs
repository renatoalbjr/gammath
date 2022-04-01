using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    void OnEnable(){
        EventManager.Instance.OnGameOver += EndGameAnimations;
        EventManager.Instance.OnSceneUnload += SceneUnloader;
        EventManager.Instance.OnAttack += Attack;
    }

    void OnDisable(){
        EventManager.Instance.OnGameOver -= EndGameAnimations;
        EventManager.Instance.OnSceneUnload -= SceneUnloader;
        EventManager.Instance.OnAttack -= Attack;
    }
    void Start()
    {
        Debug.Log("SceneManager :: Start()");

        EventManager.Instance.StartGameStateChange();
    }

    public void EndGameAnimations(){
        Debug.Log("End Game Animations");
        EventManager.Instance.StartGameStateChange();
    }

    public void SceneUnloader(){
        Debug.Log("Unloading scene...");
        Debug.Log("Finished unloading the scene");
    }

    public void Attack(){
        Debug.Log("Attacked");
        EventManager.Instance.StartTurnStageChange();
    }
}
