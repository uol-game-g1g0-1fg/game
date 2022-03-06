using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetMasterVol : MonoBehaviour
{

	public AudioMixer mixer;

	public void SetLevel(float sliderVal)
	{
		mixer.SetFloat("MasterVol", Mathf.Log10(sliderVal)*20);
	}

}
