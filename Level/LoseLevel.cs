using UnityEngine;

/// <summary>
/// Handles losing the level, attached to anything that kills the player
/// </summary>

public class LoseLevel : MonoBehaviour
{
    #region Unity Inspector Fields

    [Tooltip("When the level is lost, this event will be raised")]
    [SerializeField]
    private GameEvent onLevelLose;

    [Tooltip("The bool variable scriptable object to set when game is over")]
    [SerializeField]
    private BoolVariable isGameOver;

    #endregion

    //Kills player when entered
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            isGameOver.SetValue(true);
            onLevelLose.Raise();
        }
    }

    /// <summary>
    /// Called by the event listener attached
    /// </summary>
    public void OnLevelStart_CallBack() {
        isGameOver.SetValue(false);
    }

    /// <summary>
    /// Toggles KillPlayer Object so that player can fall without 
    /// penalty once level is won
    /// </summary>
    public void DisableSelf()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// Enables the kill zone/box Collider to kill player
    /// </summary>
    public void EnableSelf()
    {
        GetComponent<BoxCollider>().enabled = true;
    }
}
