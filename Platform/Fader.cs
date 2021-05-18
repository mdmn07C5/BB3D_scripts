using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fader : MonoBehaviour
{
	private Color color;
	private float duration = 1f;
	// private MeshRenderer meshRenderer;
	
	private Shader shader;
	float alphaStart = 1f;
	float alphaEnd = 0f;		
    // Start is called before the first frame update
    void Start()
    {
    	shader = gameObject.GetComponent<Shader>();
    	;
        // renderer = gameObje
    }

    // Update is called once per frame
    void Update()
    {	
    	float lerp = Mathf.PingPong(Time.time, duration) / duration;
    	float newAlpha = Mathf.Lerp(alphaStart, alphaEnd, lerp);
    	Color newColor = new Color(color.r, color.g, color.b, newAlpha);
    	// shader.color = newColor;
    	print(newAlpha);
    }

    public void Fade()
    {
    	
    }
}
