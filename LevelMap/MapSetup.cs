using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Used in map level scene, sets up the map and it's levels
/// </summary>

public class MapSetup : MonoBehaviour
{
    [Tooltip("Reference to the platforms on the map")]
    [SerializeField]
    private List<GameObject> mapPlatforms = new List<GameObject>();

    private int currentLevel;

    private void Start()
    {
        Init();   
    }

    /// <summary>
    /// Sets all the platform's text to the levels
    /// </summary>
    private void Init()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        for (int i = 0; i < mapPlatforms.Count; i++)
        {
            if (mapPlatforms[i].GetComponentInChildren<TextMeshPro>())
            {
                mapPlatforms[i].GetComponentInChildren<TextMeshPro>().text = (currentLevel + i) + "";
            }
        }
    }
}
