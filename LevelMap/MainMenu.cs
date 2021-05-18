using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used in the Map Level Scene
/// </summary>

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string gameLevel;

    /// <summary>
    /// Loads our game from the map level, called by button
    /// </summary>
    public void LoadGame()
    {
        SceneManager.LoadScene(gameLevel);
    }
}
