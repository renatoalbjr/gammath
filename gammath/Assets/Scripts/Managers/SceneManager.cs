using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    #region Variables
    public static SceneManager Instance { get; private set; }
    #endregion

    // ########################################################################################## //

    #region EventManager Subscriber/Unsubscriber
    // ########################################################################################## //

    ///<summary>Horrible piece of boilerplate code to get around unity's random awkening order</summary>
    private bool isSubscribed = false;
    ///<summary>Horrible piece of boilerplate code to get around unity's random awkening order</summary>
    private bool _tryToSubscribe(){
        if(isSubscribed) return false;
        if(EventManager.Instance == null) return false;
        isSubscribed = true;

        // ---Subscribe methods to events---
        EventManager.Instance.OnGameOver += EndGameAnimations;
        EventManager.Instance.OnSceneUnload += SceneUnloader;
        EventManager.Instance.OnAttack += Attack;
        return true;
    }
    ///<summary>Horrible piece of boilerplate code to get around unity's random awkening order</summary>
    private bool _tryToUnsubscribe(){
        if(EventManager.Instance == null) return false;
        isSubscribed = false;

        // ---Unsubscribe methods to events---
        EventManager.Instance.OnGameOver -= EndGameAnimations;
        EventManager.Instance.OnSceneUnload -= SceneUnloader;
        EventManager.Instance.OnAttack -= Attack;
        return true;
    }
    #endregion

    #region Unity Methods
    #region Awake
    // ########################################################################################## //

    void Awake(){
        Debug.Log("Game Object: "+gameObject.name);
        
        isSubscribed = false;
        
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
        _tryToSubscribe();
        Debug.Log("Game Object: "+gameObject.name);
    }
    #endregion

    #region OnDisable
    // ########################################################################################## //

    void OnDisable(){
        _tryToUnsubscribe();
    }
    #endregion

    #region Start
    // ########################################################################################## //

    void Start()
    {
        _tryToSubscribe();
        Debug.Log("Game Object: "+gameObject.name);
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
