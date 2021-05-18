using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles losing the level, attached to anything that allows player to win game
/// </summary>

public class WinLevel : MonoBehaviour
{
    #region Unity Inspector Fields

    [Tooltip("An int variable to the amount of current stars in the current level")]
    [SerializeField]
    private IntVariable currentStar;

    [Tooltip("An int variable to the amount of max stars in the current level")]
    [SerializeField]
    private IntVariable maxStar;

    [Tooltip("When level is won, this event will be raised")]
    [SerializeField]
    private GameEvent onLevelWin;

    #endregion

    private static bool gameWon;

    /// <summary>
    /// Tries to win the game by comparing the amount of current stars/collectables with the max amount
    /// </summary>
    public void TryWinGame()
    {
        if (AllStarsPickedUp() && gameWon == false)
        {
            gameWon = true;
            onLevelWin.Raise();
        }
    }

    /// <summary>
    /// Resets collectable/stars
    /// </summary>
    public void Reset_Callback()
    {
        gameWon = false;
        currentStar.SetValue(0);
        maxStar.SetValue(0);
    }

    private bool AllStarsPickedUp()
    {
        return currentStar.GetValue() == maxStar.GetValue();
    }
}
