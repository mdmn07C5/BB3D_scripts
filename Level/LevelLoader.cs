using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Our script that prepares the scene and loads the level using the PlatformSpawner script
/// </summary>

[RequireComponent(typeof(PlatformSpawner))]
public class LevelLoader : MonoBehaviour
{
    #region Unity Inspector Fields

    [Tooltip("Uses level saving, can replay the saved level after you quit game, Press 'space' to reset lvls, editor only")]
    [SerializeField]
    private bool useLevelSave;

    [Tooltip("An IntReference scriptable object to the level we are loading")]
    [SerializeField]
    private IntReference levelToLoad;

    [Tooltip("A scriptable object containing a list of Texture2D as our levels")]
    [SerializeField]
    private Texture2DListVariable levels;

    [Tooltip("A scriptable object containing the list of strings of Scenes To load into the main scene to build a level")]
    [SerializeField]
    private StringListVariable prepScenes;

    [Header("[Events to Raise]")]
    [Tooltip("When the level int variable is changed, this event will be raised")]
    [SerializeField]
    private GameEvent onLevelVarChange;

    [Tooltip("When the platforms are done generating, this even will be raised")]
    [SerializeField]
    private GameEvent onPlatformGenerated;

    #endregion

    /// <summary>
    /// We are using a singleton to make sure we only have one level loader when we combine and prepare the scenes during runtime
    /// </summary>
    private static LevelLoader instance;

    private PlatformSpawner platformSpawner;
    private PlatformTransition levelFloatUp; //The level/platform floating up/down animation when level starts/ends
    private bool buildFinish; //used to check if we finish building the levels

    #region Unity Callbacks

    private void Awake()
    {
        Application.targetFrameRate = 60;
        //Our singleton pattern
        if (instance != null && instance != this)
        {
            // destroy the gameobject if an instance of this exist already
            Destroy(gameObject);
        }
        else
        {
            //Set our instance to this object/instance
            Application.targetFrameRate = 60;
            instance = this;
        }

        platformSpawner = GetComponent<PlatformSpawner>();
        levelFloatUp = GetComponent<PlatformTransition>();
    }

    private void Start()
    {
        //if we are using level saving, we will get the saved level 
        if(useLevelSave)
            GetSavedLevel();

        //Prepare and load level
        PrepareScene();
        LoadLevel();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ResetLevel();
    }
#endif

    #endregion

    /// <summary>
    /// Increases the current level and raises the onLevelVarChange event then calls LoadLevel
    /// </summary>
    public void LoadNextLevel()
    {
        levelToLoad.Variable.Add(1);
        onLevelVarChange.Raise();
        LoadLevel();
    }

    /// <summary>
    /// Reloads the same level
    /// </summary>
    public void RestartLevel()
    {
        LoadLevel();
    }

    /// <summary>
    /// Build our scene/level from prep scenes, called once when game starts in order to put together the scenes
    /// </summary>
    private void PrepareScene()
    {
        StartCoroutine("PrepareSceneCo");
    }

    /// <summary>
    /// Load the current level
    /// </summary>
    private void LoadLevel()
    {
        SaveLevel();
        StartCoroutine("LoadLevelCo");
    }

    /// <summary>
    /// The coroutine to prepare the level, goes through the list of prep scenes and loads them in
    /// </summary>
    /// <returns></returns>
    private IEnumerator PrepareSceneCo()
    {
        for (int i = 0; i < prepScenes.list.Count; i++)
        {
            yield return StartCoroutine(LoadScene(prepScenes.list[i]));
            yield return null;
        }
        buildFinish = true;
    }

    /// <summary>
    /// Loads the level by generating the platforms needed for the current level
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadLevelCo()
    {
        while (!buildFinish)
            yield return null;

        platformSpawner.SetPlatformImage(levels.list[levelToLoad.Value - 1]);
        platformSpawner.GeneratePlatform(PlatformType.ColorMapping);
        onPlatformGenerated.Raise();
    }

    #region Scene Management

    /// <summary>
    /// Loads the scene into the currently running scene
    /// </summary>
    /// <param name="sceneName">Scene to load</param>
    /// <returns></returns>
    private IEnumerator LoadScene(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// Unloads the scene from the current scene
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator UnloadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

            while (!asyncUnload.isDone)
            {
                yield return null;
            }

        }
    }

    #endregion Scene Management

    /// <summary>
    /// Sets the levelToLoad IntReference to our saved level in PlayerPrefs
    /// </summary>
    private void GetSavedLevel()
    {
        levelToLoad.Variable.SetValue(PlayerPrefs.GetInt("CurrentLevel", 1));
    }

    /// <summary>
    /// Save our IntReference of currentlevel to the playerPref
    /// </summary>
    private void SaveLevel()
    {
        PlayerPrefs.SetInt("CurrentLevel", levelToLoad.Variable.GetValue());
    }

    /// <summary>
    /// Resets the level 1 and saves it
    /// </summary>
    public void ResetLevel()
    {
        levelToLoad.Variable.SetValue(1);
        SaveLevel();
    }
}
