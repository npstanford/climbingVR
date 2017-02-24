using UnityEngine;
using UnityEngine.UI; //used for stepping so can remove if that is no longer here

using System.Collections;

public class Blink : MonoBehaviour {
    public Image BlinkMask;
    public float flashSpeed;                               // The speed the damageImage will fade at.
    public Color flashColor;     // The colour the damageImage is set to, 
                                                             // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void BlinkScreen(Color flashColor, float flashSpeed)
    {
        IEnumerator coroutine;
        coroutine = BlinkScreenCoroutine(flashColor, flashSpeed);
        StartCoroutine(coroutine);
    }

    IEnumerator BlinkScreenCoroutine(Color flashColor, float flashSpeed)
    {
        BlinkMask.color = flashColor;
        BlinkMask.color = Color.Lerp(BlinkMask.color, Color.clear, flashSpeed * Time.deltaTime);
        yield return null;
    }
}
