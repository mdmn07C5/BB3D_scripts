using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all the platforms that makes up a level, destroys platform on level win/lose
/// </summary>

public class PlatformManager : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private GameEvent onVictoryDestroy;

    #endregion

    [HideInInspector]
    public List<GameObject> normalPlatforms = new List<GameObject>();

    [HideInInspector]
    public List<GameObject> victoryPlatforms = new List<GameObject>();

    [HideInInspector]
    public Transform startingAnchor;

    [HideInInspector]
    public Transform endAnchor;

    private PlatformTransition transition;

    private void Awake()
    {
        transition = GetComponent<PlatformTransition>();
    }

    public void DisableVictoryPlatforms()
    {
        for (int i = 0; i < victoryPlatforms.Count; i++)
        {
            Destroy(victoryPlatforms[i].GetComponent<WinLevel>());
        }
    }

    public void DestroyNormalPlatforms()
    {
        for (int i = 0; i < normalPlatforms.Count; i++)
        {
            StartCoroutine(transition.FloatDownCo(normalPlatforms[i]));
        }
    }

    public void TryDestroyVictoryPlatforms()
    {
        if (victoryPlatforms.Count == 0)
            return;

        if (victoryPlatforms[0].GetComponent<WinLevel>() != null)
            return;

        for (int i = 0; i < victoryPlatforms.Count; i++)
        {
            StartCoroutine(transition.FloatDownCo(victoryPlatforms[i]));
        }

        onVictoryDestroy.Raise();
        Destroy(gameObject, 1f);
    }

    public void AddPlatformAsMoveTarget()
    {
        PlayerInput input = FindObjectOfType<PlayerInput>();
        input.SetTarget(transform.root);
    }
}