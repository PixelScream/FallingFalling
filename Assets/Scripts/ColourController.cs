using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ColourController : MonoBehaviour {
	public float hue = 200;
	public float sat = 0.986f;
	public float value = 0.986f;

	public float envSat = 0.5f;
	public float envValue = 0.5f;
	public float envHueOffset = 60;

	public Color heroColor = new Color(0.25f, 0.78f, 0.45f, 1);
	public GameObject[] tiles;
	private SpriteRenderer[] sprites;
	public Slider hueSlider;

	void Start () {
		sprites = tiles [0].transform.GetComponentsInChildren<SpriteRenderer> ();
		hueSlider.value = hue / 360;
	}

	void Update () {

	}

	public void ChangeColour () {
		Debug.Log ("changed");
		hue = hueSlider.value * 360;
		heroColor = FromAhsb (1, hue, sat, value);

		for(int i = 0; i < sprites.Length; i++ ) {
			sprites[i].color = heroColor;
		}

		// grey tiles
		SpriteRenderer[] envSprites = tiles [1].GetComponentsInChildren<SpriteRenderer> ();
		float envHue = hue - envHueOffset;
		if (hue < envHueOffset) {
			envHue += 360;
			Debug.Log("below offset");
		}
		Color envColor = FromAhsb (1, envHue, envSat, envValue);	
		for(int i = 0; i < envSprites.Length; i++ ) {
			envSprites[i].color = envColor;
		}

		// white tiles
		envSprites = tiles [2].GetComponentsInChildren<SpriteRenderer> ();
		envHue = hue + envHueOffset;
		if (hue > (360 - envHueOffset)) {
			envHue -= 360;
			Debug.Log("above offset");
		}
		envColor = FromAhsb (1, envHue, envSat, envValue);	
		for(int i = 0; i < envSprites.Length; i++ ) {
			envSprites[i].color = envColor;
		}

		// enemy
		envSprites = tiles [3].GetComponents<SpriteRenderer> ();
		envHue = hue - 180;
		if (hue < 180) {
			envHue += 360;
		}
		envColor = FromAhsb (1, envHue, sat, value);	
		for(int i = 0; i < envSprites.Length; i++ ) {
			envSprites[i].color = envColor;
		}
	}

	public static Color FromAhsb(int alpha, float hue, float saturation, float brightness)
	{
		float fMax, fMid, fMin;
		int iSextant, iMax, iMid, iMin;
		
		if (0.5 < brightness)
		{
			fMax = brightness - (brightness * saturation) + saturation;
			fMin = brightness + (brightness * saturation) - saturation;
		}
		else
		{
			fMax = brightness + (brightness * saturation);
			fMin = brightness - (brightness * saturation);
		}
		
		iSextant = (int)Mathf.Floor(hue / 60f);
		if (300f <= hue)
		{
			hue -= 360f;
		}
		
		hue /= 60f;
		hue -= 2f * (float)Mathf.Floor(((iSextant + 1f) % 6f) / 2f);
		if (0 == iSextant % 2)
		{
			fMid = (hue * (fMax - fMin)) + fMin;
		}
		else
		{
			fMid = fMin - (hue * (fMax - fMin));
		}
		/*
		iMax = Convert.ToInt32(fMax * 255);
		iMid = Convert.ToInt32(fMid * 255);
		iMin = Convert.ToInt32(fMin * 255);
		*/
		
		switch (iSextant)
		{
		case 1:
			return new Color( fMid, fMax, fMin, alpha);
		case 2:
			return new Color( fMin, fMax, fMid, alpha);
		case 3:
			return new Color( fMin, fMid, fMax, alpha);
		case 4:
			return new Color( fMid, fMin, fMax, alpha);
		case 5:
			return new Color( fMax, fMin, fMid, alpha);
		default:
			return new Color( fMax, fMid, fMin, alpha);
		}
	}
}

