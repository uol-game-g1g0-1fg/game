using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetAmbientSFX : MonoBehaviour
{

	public AudioMixer mixer;

	public void SetLevel(float sliderVal)
	{
		mixer.SetFloat("AmbientVol", Mathf.Log10(sliderVal) * 20);
	}

}
