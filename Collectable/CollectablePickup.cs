using UnityEngine;

/// <summary>
/// Attached to the collectables/stars in the game
/// Handles the pickup and events that happens after the pickup
/// </summary>

public class CollectablePickup : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private int scoreToAdd;

    [Tooltip("An int reference to the amount of current stars in the current level")]
    [SerializeField]
    private IntReference curStars;

    [Tooltip("An int reference to the amount of max stars in the current level")]
    [SerializeField]
    private IntReference maxStars;

    [Tooltip("When a star is collected, this event will be raised")]
    [SerializeField]
    private GameEvent onStarCollect;

    [Tooltip("The particle fx to be spawned when its picked up")]
    [SerializeField]
    private GameObject fxToSpawn;

    #endregion

    private float maxRayCastDist = 1.5f;
    private RaycastHit[] hitBuffer;
    private GameObject platform;


    #region Unity Callbacks

    private void Start()
    {
        //Adds a collectable amount to the maxstars
        maxStars.Variable.Add(1);

        //gets the platform reference that this collectable is on top of
        GetPlatform();    	
    }

    private void OnTriggerEnter(Collider other)
    {
        //we will pick up when player is collided
        if (other.tag == "Player")
        {
            PickUp();
        }
    }

    #endregion

    /// <summary>
    /// When the collectable is picked up it 
    /// </summary>
    public void PickUp()
    {
        //adds 1  to the current stars
        curStars.Variable.Add(1);

        // raise the event
        onStarCollect.Raise();

        //Spawns the score
        GameObject obj = ObjectPooler.instance.SpawnFromPool("ScorePopupText");
        obj.GetComponent<ScorePopup>().Init(scoreToAdd);

        //Spawn fx
        Instantiate(fxToSpawn, transform);


        Destroy(gameObject);
    }

    /// <summary>
    /// gets the platform that this collectable is on top of
    /// </summary>
    private void GetPlatform()
    {
        hitBuffer = Physics.RaycastAll(transform.position, Vector3.down, maxRayCastDist);
        foreach (RaycastHit hit in hitBuffer)
        {
            if (hit.collider.gameObject.tag == "platform")
                platform = hit.collider.gameObject;
        }
    }
}
