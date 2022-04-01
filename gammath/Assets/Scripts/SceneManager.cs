using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    #region Variables
    public static SceneManager Instance;
    #endregion

    // ########################################################################################## //

    #region Unity Methods
    #region Awake
    // ########################################################################################## //

    void Awake(){
        // ---Singleton Implementation---
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    #endregion
    
    #region OnEnable
    // ########################################################################################## //

    void OnEnable(){
        EventManager.Instance.OnGameOver += EndGameAnimations;
        EventManager.Instance.OnSceneUnload += SceneUnloader;
        EventManager.Instance.OnAttack += Attack;
    }
    #endregion

    #region OnDisable
    // ########################################################################################## //

    void OnDisable(){
        EventManager.Instance.OnGameOver -= EndGameAnimations;
        EventManager.Instance.OnSceneUnload -= SceneUnloader;
        EventManager.Instance.OnAttack -= Attack;
    }
    #endregion

    #region Start
    // ########################################################################################## //

    void Start()
    {
        Debug.Log("SceneManager :: Start()");

        EventManager.Instance.StartGameStateChange();
    }
    #endregion

    #region Update
    // ########################################################################################## //


    #endregion
    #endregion

    // ########################################################################################## //

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
