using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    #region Variables
    public static SceneLoader Instance { get; private set; }
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Image _progress;
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

        //EventManager.Instance.StartGameStateChange();
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

    #region Actual SceneLoading
    public async void LoadScene(string sceneName){
        var scene = SceneManager.LoadSceneAsync(sceneName);
        
        scene.allowSceneActivation = false;
        _loaderCanvas.SetActive(true);
        
        do
        {
            await Task.Delay(100);
            _progress.fillAmount = scene.progress;
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;
        _loaderCanvas.SetActive(false);

        if(sceneName == "BattleScene"){
            _tryToSubscribe();
            EventManager.Instance?.StartGameStateChange();
        }

    }
    #endregion
}
