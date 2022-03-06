using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetPlayerSFX : MonoBehaviour
{

	public AudioMixer mixer;

	public void SetLevel(float sliderVal)
	{
		mixer.SetFloat("PlayerVol", Mathf.Log10(sliderVal) * 20);
	}

}
