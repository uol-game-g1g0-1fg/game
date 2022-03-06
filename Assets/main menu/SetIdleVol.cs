using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetIdleVol : MonoBehaviour
{

	public AudioMixer mixer;

	public void SetLevel(float sliderVal)
	{
		mixer.SetFloat("IdleVol", Mathf.Log10(sliderVal) * 20);
	}

}
