using System.Collections;
using UnityEngine;

/// <summary>
/// Controls each individual platform pieces, contains jiggling and impact
/// </summary>

public class PlatformController : MonoBehaviour
{
    #region Unity Inspector Fields 

    [Tooltip("How many hit/impacts should this platform platform have before it sinks/dies")]
    [SerializeField] 
	private int hitPoints;
    
    [SerializeField]
	private VerticalJiggle vjiggle;

    [SerializeField]
    private Buoy buoy;

    [Tooltip("The fx to spawn when platform is being impacted")]
    [SerializeField]
    private GameObject splashPrefab;

    [Tooltip("The collectable child gameobject reference")]
    [SerializeField]
    private GameObject starCollectible;

    #endregion

    #region When Ball Hits Brick
    public void CallJiggle()
    {
        StopCoroutine("OnImpact");
        StartCoroutine("OnImpact");
    }

    IEnumerator OnImpact()
    {
        Instantiate(splashPrefab, transform);


        yield return StartCoroutine(vjiggle.Jiggle(gameObject.transform));
        yield return StartCoroutine(TakeHealth());
    }

    IEnumerator TakeHealth()
    {
    	if ( --hitPoints < 1 ) {
            GivePoints();
            yield return StartCoroutine(buoy.Sink(gameObject.transform));
    		Break();
    	}

    	yield return null;
    }

    void Break()
    {
        if (starCollectible != null)
        {
            starCollectible.GetComponent<CollectablePickup>().PickUp();
        }
        Destroy(gameObject);
    }

    void GivePoints() {
        GameObject obj = ObjectPooler.instance.SpawnFromPool("ScorePopupText");
        obj.GetComponent<ScorePopup>().Init(10);
    }

    #endregion

}
