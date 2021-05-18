using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles sinking a gameobject 
/// </summary>

[System.Serializable]
public class Buoy
{
	[SerializeField]
	[Tooltip("How fast the object will sink")]
	private float sinkSpeed = 75f;
	[SerializeField]
	[Tooltip("How long it will take the object to sink before it halts")]
	private float timeToLive = 1f;

    public IEnumerator Sink(Transform t)
    {
    	float timeElapsed = 0f;
    	Vector3 originPosition = t.position;
    	while ( timeElapsed < timeToLive ) {
    		Vector3 nextPos = t.position;
    		nextPos.y = originPosition.y - (sinkSpeed * timeElapsed);
    		timeElapsed +=  Time.deltaTime;
    		t.position = nextPos;
    		yield return null;
    	}
    }
}
